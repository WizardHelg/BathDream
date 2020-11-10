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

        IWebHostEnvironment _env;
        public string Title { get; set; } = "Тестируем";

        [BindProperty]
        public Order Order { get; set; }

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
                FillWorkBook(wb, Order);
                
                do
                {
                    path = GetFreePath();
                } while (System.IO.File.Exists(path));

                wb.SaveAs(path);
            }

            if (string.IsNullOrEmpty(path)) return null;

            return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Заказ.xlsx");
        }

        private XLWorkbook FillWorkBook(XLWorkbook wb, Order order)
        {
            var ws = wb.Worksheet(1);
            List<string> del_rows = new List<string>();
            bool del_section_flag;

            ws.Range("G3").Value = DateTime.Now.ToShortDateString();

            if (order.RequiredRemoval)
                ws.Cell("G18").Value = ws.Cell("H11").Value;
            else
                del_rows.Add("17:18");

            if (order.FloorType.ToLower() == "плитка")
            {
                var value = ws.Cell("H11").Value;
                for (int row = 20; row < 26; row++)
                    ws.Cell($"G{row}").Value = value;
            }
            else
                del_rows.Add("19:27");

            if(order.WallCoverType.ToLower() == "плитка")
            {
                var value = ws.Cell("H12").Value;
                for (int row = 29; row < 35; row++)
                    ws.Cell($"G{row}").Value = value;
            }
            else
                del_rows.Add("28:36");

            del_section_flag = true;
            if (order.SwitchAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G38").Value = order.SwitchAmount;
            }                   
            else
                del_rows.Add("38");

            if (order.WarmFloor)
            {
                del_section_flag = false;
                ws.Cell("G39").Value = 2;
                ws.Cell("G40").Value = ws.Cell("H12").Value;
            }
            else
                del_rows.Add("39:40");

            if (del_section_flag)
                del_rows.Add("37");

            del_section_flag = true;
            if(order.BathAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G42").Value = order.BathAmount;
            }
            else
                del_rows.Add("42");

            if (order.ShowerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G43").Value = order.ShowerAmount;
            }
            else
                del_rows.Add("43");

            if(order.ShowerConerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G44").Value = order.ShowerConerAmount;
            }
            else
                del_rows.Add("44");

            if (order.JacuzziAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G45").Value = order.JacuzziAmount;
            }
            else
                del_rows.Add("45");

            if (order.HydroBathAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G46").Value = order.HydroBathAmount;
            }
            else
                del_rows.Add("46");

            if (order.ToiletAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G47").Value = order.ToiletAmount;
            }
            else
                del_rows.Add("47");

            if(order.InstallationAndToiletAmount > 0 || order.InstallationAndBidetAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G48").Value = order.InstallationAndToiletAmount + order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add("48");

            if (order.InstallationAndToiletAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G49").Value = order.InstallationAndToiletAmount;
            }
            else
                del_rows.Add("49");

            if (order.BidetAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G50").Value = order.BidetAmount;
            }
            else
                del_rows.Add("50");

            if (order.InstallationAndBidetAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G51").Value = order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add("51");

            if (order.HygienicShowerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G52").Value = order.HygienicShowerAmount;
            }
            else
                del_rows.Add("52");

            if (order.SinkAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G53").Value = order.SinkAmount;
            }
            else
                del_rows.Add("53");

            if (order.BedsideAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G54").Value = order.BedsideAmount;
            }
            else
                del_rows.Add("54");

            if (order.MirrorAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G55").Value = order.MirrorAmount;
            }
            else
                del_rows.Add("55");

            if (order.TowelDryerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G56").Value = order.TowelDryerAmount;
            }
            else
                del_rows.Add("56");

            if (order.BathroomAccessoriesAmount > 0)
            {
                del_section_flag = false;
                ws.Cell("G57").Value = order.BathroomAccessoriesAmount;
            }
            else
                del_rows.Add("57");

            if (del_section_flag)
                del_rows.Add("41");

            switch (order.CeilingCoverType.ToLower())
            {
                case "реечный":
                    del_rows.Add("60:61");
                    ws.Cell("G59").Value = ws.Cell("H13").Value;
                    break;
                case "натяжной":
                    del_rows.Add("59");
                    del_rows.Add("61");
                    ws.Cell("G60").Value = ws.Cell("H13").Value;
                    break;
                case "окраска":
                    del_rows.Add("59:60");
                    ws.Cell("G61").Value = ws.Cell("H13").Value;
                    break;
                default:
                    del_rows.Add("58:61");
                    break;
            }

            var quere = from item in del_rows
                        let fe = int.Parse(item.Split(':', 2)[0])
                        orderby fe ascending
                        select item;

            string temp = String.Join(',', quere);
            ws.Rows(temp).Delete();

            //нумерация
            int ptr = 17;
            int index = 1;
            while(ws.Cell(ptr, 4).Value.ToString().ToLower() != "итог")
            {
                ptr++;
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
