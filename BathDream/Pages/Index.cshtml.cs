using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                Image ="img/Bath.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Душевая кабина",
                BindedProperty = "Order.ShowerAmount",
                Image ="img/Shower.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Душевой уголок",
                BindedProperty = "Order.ShowerConerAmount",
                Image ="img/ShowerConer.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Джакузи",
                BindedProperty = "Order.JacuzziAmount",
                Image ="img/Jacuzzi.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Ванна с функцией гидромассажа",
                BindedProperty = "Order.HydroBathAmount",
                Image ="img/HydroBath.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Унитаз",
                BindedProperty = "Order.ToiletAmount",
                Image ="img/Toilet.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция + подвесной унитаз",
                BindedProperty = "Order.InstallationAndToiletAmount",
                Image ="img/InstallationAndToilet.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Инсталляция биде + подвесное биде",
                BindedProperty = "Order.InstallationAndBidetAmount",
                Image ="img/InstallationAndBidet.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Гигиенический душ",
                BindedProperty = "Order.HygienicShowerAmount",
                Image ="img/HygienicShower.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Биде",
                BindedProperty = "Order.BidetAmount",
                Image ="img/Bidet.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Раковина",
                BindedProperty = "Order.SinkAmount",
                Image ="img/Sink.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Тумба",
                BindedProperty = "Order.BedsideAmount",
                Image ="img/Bedside.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Зеркало",
                BindedProperty = "Order.MirrorAmount",
                Image ="img/Mirror.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Полотенцесушитель",
                BindedProperty = "Order.TowelDryerAmount",
                Image ="img/TowelDryer.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Ванные принадлежности",
                BindedProperty = "Order.BathroomAccessoriesAmount",
                Image ="img/BathroomAccessories.jpg"
            },

            new BathroomItem()
            {
                DisplyName ="Сантехнический люк",
                BindedProperty = "Order.PlumbingHatch",
                Image ="img/Plumbing.jpg"
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
                Image ="img/PLOverhead.jpg"
            },
            new BathroomItem()
            {
                DisplyName ="Фильр грубой очистки",
                BindedProperty = "Order.PLCoarseFilter",
                Image ="img/PLCoarseFilter.jpg"
            },
            new BathroomItem()
            {
                DisplyName ="Обратный клапан",
                BindedProperty = "Order.PLCheckValve",
                Image ="img/PLCheckValve.jpg"
            },
            new BathroomItem()
            {
                DisplyName ="Счётчик",
                BindedProperty = "Order.PLCounter",
                Image ="img/PLCounter.jpg"
            },
            new BathroomItem()
            {
                DisplyName ="Водяной коллектор ХВ",
                BindedProperty = "Order.PLWaterCollectorCW",
                Image ="img/PLWaterCollectorCW.jpg"
            },
            new BathroomItem()
            {
                DisplyName ="Водяной коллектор ГВ",
                BindedProperty = "Order.PLWaterCollectorHW",
                Image ="img/PLWaterCollectorCW.jpg"
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
            string order_number = "";
            using (var wb = new XLWorkbook(Path.Combine(_env.WebRootPath, "templates", "BDTemplate.xlsx")))
            {
                var ws = wb.Worksheet(1);
                
                int fill_start_row = SetRoomsTable(ws); //по дефолту должен вернуть 17 строку
                FillWorkBook(wb, Rooms, fill_start_row); //Переисать, что бы можно было начинать со любой строки
                
                do
                {
                    //try
                    //{
                        path = GetFreePath();
                    //}
                    //catch(Exception e)
                    //{
                    //    return new ContentResult() { Content = $"{e.Message}\n{e.StackTrace}" };
                    //}
                } while (System.IO.File.Exists(path));

                order_number = Path.GetFileNameWithoutExtension(path);
                FillHeader(ws, order_number);
                wb.SaveAs(path);
            }

            if (string.IsNullOrEmpty(path)) return null;

            
            StringBuilder builder = new StringBuilder();
            builder.Append($"<h2>{PartOfDay()}, {Customer.Name}.</h2>");
            builder.Append($"<p>Ваш заказ {order_number} уже в обработке. Наши операторы свяжутся с Вами по номеру {Customer.Phone} для уточнения заказа.</p>");
            builder.Append($"<p>Спасибо, что воспользовались услугами нашей компании!</P>");

            await SendEmail(Customer.Email,
                $"Документы по заказу {order_number}",
                builder.ToString(),
                new List<string>()
                {
                    path,
                    Path.Combine(_env.WebRootPath, "templates", "contract.docx")
                });
#if !DEBUG
            builder = new StringBuilder();
            builder.Append($"<h2>Заказ номер: {order_number}.</h2>");
            builder.Append($"<p>Клиент: {Customer.Name} {Customer.SurName}.</p>");
            builder.Append($"<p>E-mail: {Customer.Email}<br />Тел.: {Customer.Phone}</P>");

            await SendEmail("info@bath-dream.ru",
                $"Документы по заказу {order_number}",
                builder.ToString(),
                new List<string>()
                {
                    path
                });

#endif
            return RedirectToPagePreserveMethod("OrderResult"); //PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Заказ.xlsx");
        }

        private static string PartOfDay()
        {
            int h = DateTime.Now.Hour;
            if (h >= 6 && h < 12) return "Доброе утро";
            else if (h >= 12 && h < 18) return "Добрый день";
            else return "Добрый вечер";
        }

        private static async Task SendEmail(string email, string subject, string message, List<string> attachments)
        {
            //яндекс order@bath-dream.ru пароль BathDream2021
            var email_message = new MimeMessage();

            email_message.From.Add(new MailboxAddress("BathDream", "order@bath-dream.ru"));
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
            await smtp.ConnectAsync("smtp.yandex.ru", 465, true);
            await smtp.AuthenticateAsync("order@bath-dream.ru", "BathDream2021");
            await smtp.SendAsync(email_message);
            await smtp.DisconnectAsync(true);
        }

        private void FillHeader(IXLWorksheet ws, string order_number)
        {
            ws.Range("G3").Value = DateTime.Now.ToShortDateString();
            ws.Range("D5").Value = $"Сметный расчет: {order_number}";
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

                double ceilingArea = room.CeilingArea(),
                        floorArea = room.FloorArea(), 
                        wallsArea = room.WallsArea(), 
                        doorArea = room.Door.Area();

                sumAreas.SumCeilingArea += ceilingArea;
                sumAreas.SumFloorArea += floorArea;
                sumAreas.SumWallsArea += wallsArea;

                ws.Range($"H{pointer + 1}").Value = floorArea;
                ws.Range($"H{pointer + 2}").Value = wallsArea;
                ws.Range($"H{pointer + 3}").Value = ceilingArea;
                ws.Range($"H{pointer + 4}").Value = doorArea;

                pointer += 6;
            }

            return insert_row + 2;
        }

        private XLWorkbook FillWorkBook(XLWorkbook wb, ISumAreas sum_areas, int fill_start_row)
        {
            var ws = wb.Worksheet(1);
            List<string> del_rows = new List<string>();
            bool del_section_flag;
            int water_dots = 0;
            int sewer_dots = 0;
            double total = 0;
            
            int cursor_row = fill_start_row + 1; //18
            void SetDoubleValue(double value, int shift = 0)
            {
                ws.Cell($"G{cursor_row + shift}").Value = value;
                ws.Cell($"F{cursor_row + shift}").TryGetValue(out double res);
                total += res * value;
                ws.Cell($"H{cursor_row + shift}").Value = res * value;
            }

            void SetIntValue(int value, int shift = 0)
            {
                ws.Cell($"G{cursor_row + shift}").Value = value;
                ws.Cell($"F{cursor_row + shift}").TryGetValue(out int res);
                total += res * value;
                ws.Cell($"H{cursor_row + shift}").Value = res * value;
            }


            if (Order.RequiredRemoval)
                SetDoubleValue(sum_areas.SumFloorArea);
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row}");

            cursor_row += 2; //20
            if (Order.FloorType.ToLower() == "плитка")
            {
                double temp = sum_areas.SumFloorArea;
                for (int row = 0; row < 6; row++)
                    SetDoubleValue(temp, row);
            }
            else
                del_rows.Add($"{cursor_row - 1}:{cursor_row + 7}");

            del_section_flag = true;
            cursor_row += 9; //29
            if (Order.WallCoverType.ToLower() == "плитка")
            {
                double temp = sum_areas.SumWallsArea;
                del_section_flag = false;
                for (int row = 0; row < 5; row++)
                    SetDoubleValue(temp, row);
            }
            else
                del_rows.Add($"{cursor_row}:{cursor_row + 6}");

            //Тут панели ПВХ
            cursor_row += 7; //36
            if (Order.WallCoverType.ToLower() == "панели пвх")
            {
                del_section_flag = false;
                SetDoubleValue(sum_areas.SumWallsArea);
            }
            else
                del_rows.Add($"{cursor_row}");

            if (del_section_flag)
                del_rows.Add($"{cursor_row - 8}");

            cursor_row += 2; //38
            del_section_flag = true;
            if (Order.SwitchAmount > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.SwitchAmount);
            }                   
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //39
            if (Order.WarmFloor)
            {
                del_section_flag = false;
                SetIntValue(2);
                SetDoubleValue(sum_areas.SumFloorArea / 2, 1);
            }
            else
                del_rows.Add($"{cursor_row}:{cursor_row + 1}");

            if (del_section_flag)
                del_rows.Add($"{cursor_row - 2}");

            cursor_row += 3; //42
            del_section_flag = true;
            if(Order.BathAmount > 0)
            {
                water_dots += 2 * Order.BathAmount;
                sewer_dots += 1 * Order.BathAmount;
                del_section_flag = false;
                SetIntValue(Order.BathAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //43
            if (Order.ShowerAmount > 0)
            {
                water_dots += 2 * Order.ShowerAmount;
                sewer_dots += 1 * Order.ShowerAmount;
                del_section_flag = false;
                SetIntValue(Order.ShowerAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //44
            if (Order.ShowerConerAmount > 0)
            {
                water_dots += 2 * Order.ShowerConerAmount;
                sewer_dots += 1 * Order.ShowerConerAmount;
                del_section_flag = false;
                SetIntValue(Order.ShowerConerAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //45
            if (Order.JacuzziAmount > 0)
            {
                water_dots += 2 * Order.JacuzziAmount;
                sewer_dots += 1 * Order.JacuzziAmount;
                del_section_flag = false;
                SetIntValue(Order.JacuzziAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //46
            if (Order.HydroBathAmount > 0)
            {
                water_dots += 2 * Order.HydroBathAmount;
                sewer_dots += 1 * Order.HydroBathAmount;
                del_section_flag = false;
                SetIntValue(Order.HydroBathAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //47
            if (Order.ToiletAmount > 0)
            {
                water_dots += 1 * Order.ToiletAmount;
                sewer_dots += 1 * Order.ToiletAmount;
                del_section_flag = false;
                SetIntValue(Order.ToiletAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //48
            if (Order.InstallationAndToiletAmount > 0 || Order.InstallationAndBidetAmount > 0)
            {
                water_dots += 1 * (Order.InstallationAndToiletAmount + Order.InstallationAndBidetAmount);
                del_section_flag = false;
                SetIntValue(Order.InstallationAndToiletAmount + Order.InstallationAndBidetAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //49
            if (Order.InstallationAndToiletAmount > 0)
            {
                water_dots += 1 * Order.InstallationAndToiletAmount;
                sewer_dots += 1 * Order.InstallationAndToiletAmount;
                del_section_flag = false;
                SetIntValue(Order.InstallationAndToiletAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //50
            if (Order.BidetAmount > 0)
            {
                water_dots += 1 * Order.BidetAmount;
                sewer_dots += 1 * Order.BidetAmount;
                del_section_flag = false;
                SetIntValue(Order.BidetAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //51
            if (Order.InstallationAndBidetAmount > 0)
            {
                water_dots += 1 * Order.InstallationAndBidetAmount;
                sewer_dots += 1 * Order.InstallationAndBidetAmount;
                del_section_flag = false;
                SetIntValue(Order.InstallationAndBidetAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //52
            if (Order.HygienicShowerAmount > 0)
            {
                water_dots += 2 * Order.HygienicShowerAmount;
                sewer_dots += 1 * Order.HygienicShowerAmount;
                del_section_flag = false;
                SetIntValue(Order.HygienicShowerAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //53
            if (Order.SinkAmount > 0)
            {
                water_dots += 2 * Order.SinkAmount;
                sewer_dots += 1 * Order.SinkAmount;
                del_section_flag = false;
                SetIntValue(Order.SinkAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //54
            if (Order.BedsideAmount > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.BedsideAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //55
            if (Order.MirrorAmount > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.MirrorAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //56
            if (Order.TowelDryerAmount > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.TowelDryerAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //57
            if (Order.BathroomAccessoriesAmount > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.BathroomAccessoriesAmount);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //58
            if (Order.PlumbingHatch > 0)
            {
                del_section_flag = false;
                SetIntValue(Order.PlumbingHatch);
            }
            else
                del_rows.Add($"{cursor_row}");

            //Тут проверка по del-flag
            if (del_section_flag)
                del_rows.Add($"{cursor_row - 17}");

            //Тут вставить херню с трубами
            del_section_flag = true;
            cursor_row += 2; //60
            if (Order.RequiredReplacePipeline && water_dots > 0)
            {
                del_section_flag = false;
                SetIntValue(water_dots);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //61
            if (Order.RequiredReplacePipeline && sewer_dots > 0)
            {
                del_section_flag = false;
                SetIntValue(sewer_dots);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //62
            if (Order.PLOverheadCrane > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLOverheadCrane);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //63
            if (Order.PLCoarseFilter > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLCoarseFilter);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //64
            if (Order.PLCheckValve > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLCheckValve);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //65
            if (Order.PLCounter > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLCounter);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //66
            if (Order.PLWaterCollectorCW > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLWaterCollectorCW);
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //67
            if (Order.PLWaterCollectorHW > 0 && Order.RequiredReplacePipeline)
            {
                del_section_flag = false;
                SetIntValue(Order.PLWaterCollectorHW);
            }
            else
                del_rows.Add($"{cursor_row}");

            //Эт опотом заменить на верный сдвиг
            if (del_section_flag)
                del_rows.Add($"{cursor_row - 8}"); //67 => 59;


            cursor_row += 2; //69
            switch (Order.CeilingCoverType.ToLower())
            {
                case "реечный":
                    del_rows.Add($"{cursor_row + 1}:{cursor_row + 2}");
                    SetDoubleValue(sum_areas.SumCeilingArea);
                    break;
                case "натяжной":
                    del_rows.Add($"{cursor_row}");
                    del_rows.Add($"{cursor_row + 2}");
                    SetDoubleValue(sum_areas.SumCeilingArea, 1);
                    break;
                case "окраска":
                    del_rows.Add($"{cursor_row}:{cursor_row + 1}");
                    SetDoubleValue(sum_areas.SumCeilingArea, 2);
                    break;
                default:
                    del_rows.Add($"{cursor_row - 1}:{cursor_row + 2}");
                    break;
            }

            //Доп не нужная фигня
            del_section_flag = true;
            cursor_row += 4; //73
            if (Order.NeedMakeWalls)
            {
                del_section_flag = false;
            }
            else
                del_rows.Add($"{cursor_row}");

            cursor_row++; //74
            if (Order.InstallDoor)
            {
                del_section_flag = false;
                SetIntValue(Rooms.Count());
            }
            else
                del_rows.Add($"{cursor_row}");

            //Эт опотом заменить на верный сдвиг
            if (del_section_flag)
                del_rows.Add($"{cursor_row - 2}");

            //Вставка итога
            cursor_row += 2;
            ws.Range($"H{cursor_row}").Value = total;

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
            string path = Path.Combine(_env.WebRootPath, "files");
            DirectoryInfo directory = new DirectoryInfo(path);
            var files = directory.EnumerateFiles();
            int order_number = (files?.Count() ?? 0) + 1;
            //int order_number = 1;
            //foreach (FileInfo file in directory.EnumerateFiles())
            //    order_number++;

            return Path.Combine(
                            _env.WebRootPath,
                            "files",
                            $"{order_number:D4}.xlsx");
            //DateTime date_time = DateTime.Now;
            //StringBuilder builder = new StringBuilder();

            //builder.Append($"{date_time.Year:D4}");
            //builder.Append($"{date_time.Month:D2}");
            //builder.Append($"{date_time.Day:D2}");
            //builder.Append($"{date_time.Hour:D2}");
            //builder.Append($"{date_time.Minute:D2}");
            //builder.Append($"{date_time.Second:D2}");
            //builder.Append($"{date_time.Millisecond:D2}");
            //builder.Append($"{new Random().Next(1, 1000):D3}");
            //builder.Append(".xlsx");

            //return Path.Combine(
            //            _env.WebRootPath,
            //            "files",
            //            builder.ToString());
        }
    }
}
