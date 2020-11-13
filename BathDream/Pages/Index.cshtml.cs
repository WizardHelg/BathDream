using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BathDream.Models;
using ClosedXML.Excel;
using ClosedXML.Excel.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages
{
    public class IndexModel : PageModel
    {
        #region BathroomEquipments
        public List<BathroomItem> BathroomEquipments = new List<BathroomItem>()
        {
            new BathroomItem()
            {
                DisplyName ="Ванная",
                BindedProperty = "BathAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Душевая кабина",
                BindedProperty = "ShowerAmount",
                Image ="img/2.png"
            },

            new BathroomItem()
            {
                DisplyName ="Душевой уголок",
                BindedProperty = "BathAmShowerConerAmountount",
                Image ="img/3.png"
            },

            new BathroomItem()
            {
                DisplyName ="Джакузи",
                BindedProperty = "JacuzziAmount",
                Image ="img/4.png"
            },

            new BathroomItem()
            {
                DisplyName ="Ванна с функцией гидромассажа",
                BindedProperty = "HydroBathAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Унитаз",
                BindedProperty = "ToiletAmount",
                Image ="img/5.png"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция + подвесной унитаз",
                BindedProperty = "InstallationAndToiletAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция биде + подвесное биде",
                BindedProperty = "InstallationAndBidetAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Гигиенический душ",
                BindedProperty = "HygienicShowerAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Биде",
                BindedProperty = "BidetAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Раковина",
                BindedProperty = "SinkAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Тумба",
                BindedProperty = "BedsideAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Зеркало",
                BindedProperty = "MirrorAmount",
                Image ="img/7.png"
            },

            new BathroomItem()
            {
                DisplyName ="Полотенцесушитель",
                BindedProperty = "TowelDryerAmount",
                Image ="img/6.png"
            },

            new BathroomItem()
            {
                DisplyName ="Ванные принадлежности",
                BindedProperty = "BathroomAccessoriesAmount",
                Image ="img/bath.png"
            }
        };
        #endregion

        IWebHostEnvironment _env;
        public string Title { get; set; } = "Тестируем";

        [BindProperty]
        public Order Order { get; set; }

        [BindProperty]
        public Rooms Rooms { get; set; }

        public IndexModel(IWebHostEnvironment env) => _env = env;
        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            
        }

        public IActionResult OnPostSubmit()
        {
            string path;
            using (var wb = new XLWorkbook(Path.Combine(_env.WebRootPath, "templates", "BDTemplate.xlsx")))
            {
                int fill_start_row = SetRoomsTable(wb, Rooms); //по дефолту должен вернуть 17 строку
                FillWorkBook(wb, Order, Rooms, fill_start_row); //Переисать, что бы можно было начинать со любой строки
                
                do
                {
                    path = GetFreePath();
                } while (System.IO.File.Exists(path));

                wb.SaveAs(path);
            }

            if (string.IsNullOrEmpty(path)) return null;

            return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Заказ.xlsx");
        }

        private int SetRoomsTable(XLWorkbook wb, Rooms rooms)
        {
            var ws = wb.Worksheet(1);

            int insert_row = 15;

            for(int i = 1; i < rooms.Count(); i++)
            {
                ws.Row(insert_row).InsertRowsBelow(6);
                ws.Range($"B{insert_row - 6}:H{insert_row - 1}").CopyTo(ws.Range($"B{insert_row}:H{insert_row + 5}"));
                insert_row += 6;
            }

            int pointer = 10;
            int index = 1;
            ISumAreas sumAreas = rooms;
            foreach(Room room in rooms)
            {
                ws.Range($"B{pointer - 1}").Value = index++;
                ws.Range($"C{pointer - 1}").Value = room.Name;
                ws.Range($"E{pointer}").Value = room.Width;
                ws.Range($"F{pointer}").Value = room.Length;
                ws.Range($"G{pointer}").Value = room.Height;

                ws.Range($"E{pointer + 4}").Value = room.Door.Width;
                ws.Range($"G{pointer + 4}").Value = room.Door.Height;

                sumAreas.SumCeilingArea += room.CeilingArea();
                sumAreas.SumFloorArea += room.FloorArea();
                sumAreas.SumWallsArea += room.WallsArea();

                pointer += 6;
            }

            return insert_row + 2;
        }

        private XLWorkbook FillWorkBook(XLWorkbook wb, Order order, ISumAreas sum_areas, int fill_start_row)
        {
            var ws = wb.Worksheet(1);
            List<string> del_rows = new List<string>();
            bool del_section_flag;
            int dots = 0;

            ws.Range("G3").Value = DateTime.Now.ToShortDateString();

            int cursor_row = fill_start_row + 1; //18
            if (order.RequiredRemoval)
                ws.Cell($"G{cursor_row}").Value = sum_areas.SumFloorArea;
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row}");

            cursor_row += 2; //20
            if (order.FloorType.ToLower() == "плитка")
            {
                for (int row = cursor_row; row < cursor_row + 6; row++)
                    ws.Cell($"G{row}").Value = sum_areas.SumFloorArea;
            }
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row + 7}");

            cursor_row += 9; //29
            if (order.WallCoverType.ToLower() == "плитка")
            {
                for (int row = cursor_row; row < cursor_row + 6; row++)
                    ws.Cell($"G{row}").Value = sum_areas.SumWallsArea;
            }
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row + 7}");

            cursor_row += 9; //38
            del_section_flag = true;
            if (order.SwitchAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.SwitchAmount;
            }                   
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //39
            if (order.WarmFloor)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = 2;
                ws.Cell($"G{cursor_row + 1}").Value = sum_areas.SumFloorArea;
            }
            else
                del_rows.Add($"{cursor_row}:{cursor_row + 1}");

            if (del_section_flag)
                del_rows.Add($"{cursor_row - 2}");

            cursor_row += 3; //42
            del_section_flag = true;
            if(order.BathAmount > 0)
            {
                dots += 2 * order.BathAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.BathAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //43
            if (order.ShowerAmount > 0)
            {
                dots += 2 * order.ShowerAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.ShowerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //44
            if (order.ShowerConerAmount > 0)
            {
                dots += 2 * order.ShowerConerAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.ShowerConerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //45
            if (order.JacuzziAmount > 0)
            {
                dots += 2 * order.JacuzziAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.JacuzziAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //46
            if (order.HydroBathAmount > 0)
            {
                dots += 2 * order.HydroBathAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.HydroBathAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //47
            if (order.ToiletAmount > 0)
            {
                dots += 1 * order.ToiletAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.ToiletAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //48
            if (order.InstallationAndToiletAmount > 0 || order.InstallationAndBidetAmount > 0)
            {
                dots += 1 * (order.InstallationAndToiletAmount + order.InstallationAndBidetAmount);
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.InstallationAndToiletAmount + order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //49
            if (order.InstallationAndToiletAmount > 0)
            {
                dots += 1 * order.InstallationAndToiletAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.InstallationAndToiletAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //50
            if (order.BidetAmount > 0)
            {
                dots += 1 * order.BidetAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.BidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //51
            if (order.InstallationAndBidetAmount > 0)
            {
                dots += 1 * order.InstallationAndBidetAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //52
            if (order.HygienicShowerAmount > 0)
            {
                dots += 2;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.HygienicShowerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //53
            if (order.SinkAmount > 0)
            {
                dots += 2;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.SinkAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //54
            if (order.BedsideAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.BedsideAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //55
            if (order.MirrorAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.MirrorAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //56
            if (order.TowelDryerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.TowelDryerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //57
            if (order.BathroomAccessoriesAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = order.BathroomAccessoriesAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            //Тут вставить херню с трубами
            cursor_row++; //58
            if (order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = dots;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //59
            if (order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Rooms.Count();
            }
            else
                del_rows.Add($"{cursor_row}");

            if (del_section_flag)
                del_rows.Add($"{cursor_row - 16}"); //57 => 41;

            cursor_row += 2; //61
            switch (order.CeilingCoverType.ToLower())
            {
                case "реечный":
                    del_rows.Add($"{cursor_row + 1}:{cursor_row + 2}");
                    ws.Cell($"G{cursor_row}").Value = sum_areas.SumCeilingArea;
                    break;
                case "натяжной":
                    del_rows.Add($"{cursor_row}");
                    del_rows.Add($"{cursor_row + 2}");
                    ws.Cell($"G{cursor_row + 1}").Value = sum_areas.SumCeilingArea;
                    break;
                case "окраска":
                    del_rows.Add($"{cursor_row}:{cursor_row + 1}");
                    ws.Cell($"G{cursor_row + 2}").Value = sum_areas.SumCeilingArea;
                    break;
                default:
                    del_rows.Add($"{cursor_row - 1}:{cursor_row + 2}");
                    break;
            }

            var quere = from item in del_rows
                        let fe = int.Parse(item.Split(':', 2)[0])
                        orderby fe ascending
                        select item;

            string temp = String.Join(',', quere);
            ws.Rows(temp).Delete();

            //нумерация
            int ptr = fill_start_row;
            int index = 1;
            while(ws.Cell(ptr, 4).Value.ToString().ToLower() != "итог")
            {
                ptr++;
                if (ptr == 1000) break;
                if(ws.Cell(ptr, 3).Value.ToString().ToLower() == "")
                    continue;
                ws.Cell(ptr, 2).Value = index++;
            }

            return wb;
        }

        private string GetFreePath()
        {
            Random random = new Random();
            DateTime date_time = DateTime.Now;
            StringBuilder builder = new StringBuilder();

            builder.Append($"{date_time.Year:D4}");
            builder.Append($"{date_time.Month:D2}");
            builder.Append($"{date_time.Day:D2}");
            builder.Append($"{date_time.Hour:D2}");
            builder.Append($"{date_time.Minute:D2}");
            builder.Append($"{date_time.Second:D2}");
            builder.Append($"{date_time.Millisecond:D2}");
            builder.Append($"{new Random().Next(1, 1000):D3}");
            builder.Append(".xlsx");

            return Path.Combine(
                        _env.WebRootPath,
                        "files",
                        builder.ToString());
        }
    }
}
