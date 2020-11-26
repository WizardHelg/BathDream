using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BathDream.Models;
using ClosedXML.Excel;
using ClosedXML.Excel.Exceptions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;

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
                BindedProperty = "Order.BathAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Душевая кабина",
                BindedProperty = "Order.ShowerAmount",
                Image ="img/2.png"
            },

            new BathroomItem()
            {
                DisplyName ="Душевой уголок",
                BindedProperty = "Order.BathAmShowerConerAmountount",
                Image ="img/3.png"
            },

            new BathroomItem()
            {
                DisplyName ="Джакузи",
                BindedProperty = "Order.JacuzziAmount",
                Image ="img/4.png"
            },

            new BathroomItem()
            {
                DisplyName ="Ванна с функцией гидромассажа",
                BindedProperty = "Order.HydroBathAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Унитаз",
                BindedProperty = "Order.ToiletAmount",
                Image ="img/5.png"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция + подвесной унитаз",
                BindedProperty = "Order.InstallationAndToiletAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция биде + подвесное биде",
                BindedProperty = "Order.InstallationAndBidetAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Гигиенический душ",
                BindedProperty = "Order.HygienicShowerAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Биде",
                BindedProperty = "Order.BidetAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Раковина",
                BindedProperty = "Order.SinkAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Тумба",
                BindedProperty = "Order.BedsideAmount",
                Image ="img/bath.png"
            },

            new BathroomItem()
            {
                DisplyName ="Зеркало",
                BindedProperty = "Order.MirrorAmount",
                Image ="img/7.png"
            },

            new BathroomItem()
            {
                DisplyName ="Полотенцесушитель",
                BindedProperty = "Order.TowelDryerAmount",
                Image ="img/6.png"
            },

            new BathroomItem()
            {
                DisplyName ="Ванные принадлежности",
                BindedProperty = "Order.BathroomAccessoriesAmount",
                Image ="img/bath.png"
            }
        };
        #endregion
        #region PiplineEquipments
        public List<BathroomItem> PipeLineEquipment = new List<BathroomItem>()
        {
            new BathroomItem()
            {
                DisplyName ="Кран перекрытия",
                BindedProperty = "Order.PLOverheadCrane",
                Image ="img/faucet.png"
            },
            new BathroomItem()
            {
                DisplyName ="Фильр грубой очистки",
                BindedProperty = "Order.PLCoarseFilter",
                Image ="img/faucet.png"
            },
            new BathroomItem()
            {
                DisplyName ="Обратный клапан",
                BindedProperty = "Order.PLCheckValve",
                Image ="img/faucet.png"
            },
            new BathroomItem()
            {
                DisplyName ="Счётчик",
                BindedProperty = "Order.PLCounter",
                Image ="img/faucet.png"
            },
            new BathroomItem()
            {
                DisplyName ="Водяной коллектор ХВ",
                BindedProperty = "Order.PLWaterCollectorCW",
                Image ="img/faucet.png"
            },
            new BathroomItem()
            {
                DisplyName ="Водяной коллектор ГВ",
                BindedProperty = "Order.PLWaterCollectorHW",
                Image ="img/faucet.png"
            }
        };
        #endregion
        readonly IWebHostEnvironment _env;

        [BindProperty]
        public Order Order { get; set; }

        [BindProperty]
        public Rooms Rooms { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        public IndexModel(IWebHostEnvironment env) => _env = env;
        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            string path;
            using (var wb = new XLWorkbook(Path.Combine(_env.WebRootPath, "templates", "BDTemplate.xlsx")))
            {
                var ws = wb.Worksheet(1);
                FillHeader(ws);
                int fill_start_row = SetRoomsTable(ws); //по дефолту должен вернуть 17 строку
                FillWorkBook(wb, Rooms, fill_start_row); //Переисать, что бы можно было начинать со любой строки
                
                do
                {
                    path = GetFreePath();
                } while (System.IO.File.Exists(path));

                wb.SaveAs(path);
            }

            if (string.IsNullOrEmpty(path)) return null;
            
            await SendEmail(Customer.Email, "Заказ", "Вы заказали заказ", new List<string>()
            {
                path,
                Path.Combine(_env.WebRootPath, "templates", "contract.docx")
            });

            return RedirectToPagePreserveMethod("OrderResult"); //PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Заказ.xlsx");
        }

        private static async Task SendEmail(string email, string subject, string message, List<string> attachments)
        {
            var email_message = new MimeMessage();

            email_message.From.Add(new MailboxAddress("BathDream", "support@ms-dev.ru"));
            email_message.To.Add(new MailboxAddress("", email));
            email_message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };
            foreach (var item in attachments)
                builder.Attachments.Add(item);

            email_message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.masterhost.ru", 465, true);
            await smtp.AuthenticateAsync("support@ms-dev.ru", "KX3-EGD-kzu-5ue");
            await smtp.SendAsync(email_message);
            await smtp.DisconnectAsync(true);
        }

        private void FillHeader(IXLWorksheet ws)
        {
            ws.Range("G3").Value = DateTime.Now.ToShortDateString();
            ws.Range("D6").Value = $"Тел. {Customer.Phone}";
            ws.Range("D7").Value = $"Email: {Customer.Email}";
        }

        private int SetRoomsTable(IXLWorksheet ws)
        {
            int insert_row = 15;

            for(int i = 1; i < Rooms.Count(); i++)
            {
                ws.Row(insert_row).InsertRowsBelow(6);
                ws.Range($"B{insert_row - 6}:H{insert_row - 1}").CopyTo(ws.Range($"B{insert_row}:H{insert_row + 5}"));
                insert_row += 6;
            }

            int pointer = 10;
            int index = 1;
            ISumAreas sumAreas = Rooms;
            foreach(Room room in Rooms)
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

        private XLWorkbook FillWorkBook(XLWorkbook wb, ISumAreas sum_areas, int fill_start_row)
        {
            var ws = wb.Worksheet(1);
            List<string> del_rows = new List<string>();
            bool del_section_flag;
            int dots = 0;

            int cursor_row = fill_start_row + 1; //18
            if (Order.RequiredRemoval)
                ws.Cell($"G{cursor_row}").Value = sum_areas.SumFloorArea;
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row}");

            cursor_row += 2; //20
            if (Order.FloorType.ToLower() == "плитка")
            {
                for (int row = cursor_row; row < cursor_row + 6; row++)
                    ws.Cell($"G{row}").Value = sum_areas.SumFloorArea;
            }
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row + 7}");

            cursor_row += 9; //29
            if (Order.WallCoverType.ToLower() == "плитка")
            {
                for (int row = cursor_row; row < cursor_row + 6; row++)
                    ws.Cell($"G{row}").Value = sum_areas.SumWallsArea;
            }
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row + 7}");

            cursor_row += 9; //38
            del_section_flag = true;
            if (Order.SwitchAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.SwitchAmount;
            }                   
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //39
            if (Order.WarmFloor)
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
            if(Order.BathAmount > 0)
            {
                dots += 2 * Order.BathAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.BathAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //43
            if (Order.ShowerAmount > 0)
            {
                dots += 2 * Order.ShowerAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.ShowerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //44
            if (Order.ShowerConerAmount > 0)
            {
                dots += 2 * Order.ShowerConerAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.ShowerConerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //45
            if (Order.JacuzziAmount > 0)
            {
                dots += 2 * Order.JacuzziAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.JacuzziAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //46
            if (Order.HydroBathAmount > 0)
            {
                dots += 2 * Order.HydroBathAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.HydroBathAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //47
            if (Order.ToiletAmount > 0)
            {
                dots += 1 * Order.ToiletAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.ToiletAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //48
            if (Order.InstallationAndToiletAmount > 0 || Order.InstallationAndBidetAmount > 0)
            {
                dots += 1 * (Order.InstallationAndToiletAmount + Order.InstallationAndBidetAmount);
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.InstallationAndToiletAmount + Order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //49
            if (Order.InstallationAndToiletAmount > 0)
            {
                dots += 1 * Order.InstallationAndToiletAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.InstallationAndToiletAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //50
            if (Order.BidetAmount > 0)
            {
                dots += 1 * Order.BidetAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.BidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //51
            if (Order.InstallationAndBidetAmount > 0)
            {
                dots += 1 * Order.InstallationAndBidetAmount;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.InstallationAndBidetAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //52
            if (Order.HygienicShowerAmount > 0)
            {
                dots += 2;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.HygienicShowerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //53
            if (Order.SinkAmount > 0)
            {
                dots += 2;
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.SinkAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //54
            if (Order.BedsideAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.BedsideAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //55
            if (Order.MirrorAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.MirrorAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //56
            if (Order.TowelDryerAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.TowelDryerAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //57
            if (Order.BathroomAccessoriesAmount > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.BathroomAccessoriesAmount;
            }
            else
                del_rows.Add($"{cursor_row}");

            //Тут проверка по del-flag
            if (del_section_flag)
                del_rows.Add($"{cursor_row - 16}");

            //Тут вставить херню с трубами
            del_section_flag = true;
            cursor_row += 2; //59
            if (Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = dots;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //60
            if (Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Rooms.Count();
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //61
            if (Order.PLOverheadCrane > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLOverheadCrane;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //62
            if (Order.PLCoarseFilter > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLCoarseFilter;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //63
            if (Order.PLCheckValve > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLCheckValve;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //64
            if (Order.PLCounter > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLCounter;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //65
            if (Order.PLWaterCollectorCW > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLWaterCollectorCW;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //66
            if (Order.PLWaterCollectorHW > 0)
            {
                del_section_flag = false;
                ws.Cell($"G{cursor_row}").Value = Order.PLWaterCollectorHW;
            }
            else
                del_rows.Add($"{cursor_row}");

            //Эт опотом заменить на верный сдвиг
            if (del_section_flag)
                del_rows.Add($"{cursor_row - 8}"); //66 => 58;


            cursor_row += 2; //68
            switch (Order.CeilingCoverType.ToLower())
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


            //Удаление ненужных строк
            DelUselessRow(ws, del_rows);

            //нумерация
            Numeric(ws, fill_start_row);

            return wb;
        }

        private static void DelUselessRow(IXLWorksheet ws, List<string> del_rows)
        {
            var quere = from item in del_rows
                        let fe = int.Parse(item.Split(':', 2)[0])
                        orderby fe ascending
                        select item;

            string temp = String.Join(',', quere);
            ws.Rows(temp).Delete();
        }

        private static void Numeric(IXLWorksheet ws, int fill_start_row)
        {
            int ptr = fill_start_row;
            int index = 1;
            while (ws.Cell(ptr, 4).Value.ToString().ToLower() != "итог")
            {
                ptr++;
                if (ptr == 1000) break;
                if (ws.Cell(ptr, 3).Value.ToString().ToLower() == "")
                    continue;
                ws.Cell(ptr, 2).Value = index++;
            }
        }

        private string GetFreePath()
        {
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
