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

using BathDream.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BathDream.Services;
using System.ComponentModel.DataAnnotations;

namespace BathDream.Pages
{
    public class IndexModel : PageModel
    {
        readonly DBApplicationaContext _db;
        private readonly EmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region BathroomEquipments
        public List<BathroomItem> BathroomEquipments = new()
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
                DisplyName = "Стиральная машина",
                BindedProperty = "Order.Washer",
                Image = "img/Washer.jpg"
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
            },
        };
        #endregion
        #region PiplineEquipments
        public List<BathroomItem> PipeLineEquipment = new()
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

        [BindProperty]
        public COrder Order { get; set; }

        [BindProperty]
        public CRooms Rooms { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(DBApplicationaContext db, EmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        public class InputModel
        {
            public string Sender { get; set; }
            [Required (ErrorMessage = "Опишите проблему")]
            public string Message { get; set; }
        }

        public IActionResult OnGet()
        {
            if (User.IsInRole("architect"))
                return RedirectToPage("/Account/Architector");
            return Page();
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            TempData["OrderId"] = (await AddOrder()).Id;
            return RedirectToPage("/Account/Customer");
        }

        private async Task<Order> AddOrder()
        {
            Order order = new()
            {
                OrderType = (Order.Type)Convert.ToInt32(Order.OrderType),
                Date = DateTime.Now.Date,
                Status = Models.Order.Statuses.Temp
            };
            await _db.Orders.AddAsync(order);

            Estimate estimate = new()
            {
                Order = order
            };
            await _db.Estimates.AddAsync(estimate);

            List<Room> rooms = new();
            List<Work> works = new();

            int position = 1;
            void AddWork(string group, string innerName, double volume)
            {
                if (_db.WorkPrices.Where(x => x.InnerName == innerName).Include(x => x.WorkType).FirstOrDefault() is WorkPrice w_price)
                { 
                    Work work = w_price;
                    work.Estimate = estimate;
                    work.Position = position++;
                    work.Group = group;
                    work.Volume = volume;
                    works.Add(work);
                }
            }

            foreach (CRoom c_room in Rooms)
                rooms.Add(new Room()
                {
                    Name = c_room.Name,
                    Width = c_room.Width,
                    Height = c_room.Height,
                    Length = c_room.Length,
                    DoorWidth = c_room.Door.Width,
                    DoorHeight = c_room.Door.Height,
                    Estimate = estimate
                });
            await _db.Rooms.AddRangeAsync(rooms);

            double sum_floor_area = rooms.Sum(x => x.FloorArea());
            double sum_wall_area = rooms.Sum(x => x.WallsArea());
            double sum_ceiling_area = rooms.Sum(x => x.CeilingArea());
            int water_dots = 0;
            int sewer_dots = 0;

            if (Order.RequiredRemoval)
                AddWork("Демонтажные работы", "ДПомещения", sum_floor_area);

            if (Order.NeedMakeWalls)
                AddWork("Возведение стен", "ВозведениеПерегородок", 0);

            if(Order.FloorType.ToLower() == "плитка")
            {
                AddWork("Пол", "Грунтовка", sum_floor_area);
                AddWork("Пол", "Стяжка5", sum_floor_area);
                AddWork("Пол", "Гидроизоляция", sum_floor_area);
                AddWork("Пол", "УкладкаПлитки", sum_floor_area);
                AddWork("Пол", "ЗатиркаПлитки", sum_floor_area);
                AddWork("Пол", "АнтигрибковоеПокрытие", sum_floor_area);
                AddWork("Пол", "ПодрезкаПлитки", 0);
                AddWork("Пол", "ЗапилПлитки", 0);
            }

            if(Order.WallCoverType.ToLower() == "плитка")
            {
                AddWork("Стены", "Грунтовка", sum_wall_area);
                AddWork("Стены", "Гидроизоляция", Math.Round(sum_wall_area * 0.8, 4));
                AddWork("Стены", "ВыравниваниеСтен", sum_wall_area);
                AddWork("Стены", "АнтигрибковоеПокрытие", sum_wall_area);
                AddWork("Стены", "УкладкаПлитки", sum_wall_area);
                AddWork("Стены", "ЗатиркаПлитки", sum_wall_area);
                AddWork("Стены", "ПодрезкаПлитки", 0);
                AddWork("Стены", "ЗапилПлитки", 0);
            }

            if (Order.WallCoverType.ToLower() == "панели пвх")
                AddWork("Стены", "ПВХПанели", sum_wall_area);

            if (Order.SwitchAmount > 0)
                AddWork("Электрика", "УстановкаРозеток", Order.SwitchAmount);

            if (Order.WarmFloor)
            {
                AddWork("Электрика", "Терморегулятор", 1);
                AddWork("Электрика", "ТеплыйПол", Math.Round(sum_floor_area / 2, 4));
            }

            if (Order.BathAmount > 0)
            {
                water_dots += 2 * Order.BathAmount;
                sewer_dots += 1 * Order.BathAmount;
                AddWork("Сантехника", "Ванная", Order.BathAmount);
            }

            if (Order.ShowerAmount > 0)
            {
                water_dots += 2 * Order.ShowerAmount;
                sewer_dots += 1 * Order.ShowerAmount;
                AddWork("Сантехника", "ДушеваяКабина", Order.ShowerAmount);
            }

            if (Order.ShowerConerAmount > 0)
            {
                water_dots += 2 * Order.ShowerConerAmount;
                sewer_dots += 1 * Order.ShowerConerAmount;
                AddWork("Сантехника", "ДушевойУголок", Order.ShowerConerAmount);
            }

            if (Order.JacuzziAmount > 0)
            {
                water_dots += 2 * Order.JacuzziAmount;
                sewer_dots += 1 * Order.JacuzziAmount;
                AddWork("Сантехника", "Джакузи", Order.JacuzziAmount);
            }

            if (Order.HydroBathAmount > 0)
            {
                water_dots += 2 * Order.HydroBathAmount;
                sewer_dots += 1 * Order.HydroBathAmount;
                AddWork("Сантехника", "ВаннаяГидромассажем", Order.HydroBathAmount);
            }

            if (Order.ToiletAmount > 0)
            {
                water_dots += 1 * Order.ToiletAmount;
                sewer_dots += 1 * Order.ToiletAmount;
                AddWork("Сантехника", "УнитазНапольный", Order.ToiletAmount);
            }

            if (Order.InstallationAndToiletAmount > 0)
            {
                water_dots += 1 * Order.InstallationAndToiletAmount;
                sewer_dots += 1 * Order.InstallationAndToiletAmount;

                if (works.Any(w => w.InnerName == "Инсталляция"))
                {
                    works.FirstOrDefault(w => w.InnerName == "Инсталляция").Volume += Order.InstallationAndToiletAmount;
                }
                else
                {
                    AddWork("Сантехника", "Инсталляция", Order.InstallationAndToiletAmount);
                }
                
                AddWork("Сантехника", "ПодвеснойУнитаз", Order.InstallationAndToiletAmount);

            }

            if (Order.InstallationAndBidetAmount > 0)
            {
                water_dots += 2 * Order.InstallationAndBidetAmount;
                sewer_dots += 1 * Order.InstallationAndBidetAmount;

                if (works.Any(w => w.InnerName == "Инсталляция"))
                {
                    works.FirstOrDefault(w => w.InnerName == "Инсталляция").Volume += Order.InstallationAndBidetAmount;
                }
                else
                {
                    AddWork("Сантехника", "Инсталляция", Order.InstallationAndBidetAmount);
                }

                AddWork("Сантехника", "ПодвесноеБиде", Order.InstallationAndBidetAmount);                
            }

            if (Order.BidetAmount > 0)
            {
                water_dots += 2 * Order.BidetAmount;
                sewer_dots += 1 * Order.BidetAmount;
                AddWork("Сантехника", "Биде", Order.BidetAmount);
            }            

            if (Order.HygienicShowerAmount > 0)
            {
                water_dots += 2 * Order.HygienicShowerAmount;
                //sewer_dots += 1 * Order.HygienicShowerAmount;
                AddWork("Сантехника", "ГигиеническийДуш", Order.HygienicShowerAmount);
            }

            if (Order.SinkAmount > 0)
            {
                water_dots += 2 * Order.SinkAmount;
                sewer_dots += 1 * Order.SinkAmount;
                AddWork("Сантехника", "Раковина", Order.SinkAmount);
            }

            if (Order.Washer > 0)
            {
                water_dots += 1 * Order.Washer;
                sewer_dots += 1 * Order.Washer;
                AddWork("Сантехника", "СтиральнаяМашина", Order.Washer);
            }

            if (Order.BedsideAmount > 0)
                AddWork("Сантехника", "ТумбаПодРаковину", Order.BedsideAmount);

            if (Order.MirrorAmount > 0)
                AddWork("Сантехника", "Зеркало", Order.MirrorAmount);

            if (Order.TowelDryerAmount > 0)
            {
                water_dots += 2 * Order.TowelDryerAmount;
                AddWork("Сантехника", "Полотенцесушитель", Order.TowelDryerAmount);
            }
                

            if (Order.BathroomAccessoriesAmount > 0)
                AddWork("Сантехника", "Аксессуары", Order.BathroomAccessoriesAmount);

            if (Order.PlumbingHatch > 0)
                AddWork("Сантехника", "СантехническийЛюк", Order.PlumbingHatch);

            if (Order.RequiredReplacePipeline && water_dots > 0)
                AddWork("Замена труб", "РазводкаТруб", water_dots);

            if (Order.RequiredReplacePipeline && sewer_dots > 0)
                AddWork("Замена труб", "РаздкаКанализации", sewer_dots);

            if (Order.RequiredReplacePipeline && Order.PLOverheadCrane > 0)
                AddWork("Замена труб", "КранПерекрытия", Order.PLOverheadCrane);

            if (Order.RequiredReplacePipeline && Order.PLCoarseFilter > 0)
                AddWork("Замена труб", "Фильтр", Order.PLCoarseFilter);

            if (Order.RequiredReplacePipeline && Order.PLCheckValve > 0)
                AddWork("Замена труб", "ОбратныйКлапан", Order.PLCheckValve);

            if (Order.RequiredReplacePipeline && Order.PLCounter > 0)
                AddWork("Замена труб", "Счетчик", Order.PLCounter);

            if (Order.RequiredReplacePipeline && Order.PLWaterCollectorCW > 0)
                AddWork("Замена труб", "КоллекторХВ", Order.PLWaterCollectorCW);

            if (Order.RequiredReplacePipeline && Order.PLWaterCollectorHW > 0)
                AddWork("Замена труб", "КоллекторГВ", Order.PLWaterCollectorHW);

            switch (Order.CeilingCoverType.ToLower())
            {
                case "реечный":
                    AddWork("Потолок", "ПотолокРеечный", sum_ceiling_area);
                    break;
                case "натяжной":
                    AddWork("Потолок", "ПотолокНатяжной", sum_ceiling_area);
                    break;
                case "окраска":
                    AddWork("Потолок", "ПотолокОкраска", sum_ceiling_area);
                    break;
                case "стеклянный":
                    AddWork("Потолок", "ПотолокСтеклянный", sum_ceiling_area);
                    break;
            }

            if (Order.InstallDoor)
                AddWork("Прочее", "УстановкаДверей", Rooms.Count());

            await _db.Works.AddRangeAsync(works);
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<IActionResult> OnPostSendRequest(List<IFormFile> formFiles)
        {
            List<string> attachments = new List<string>();
            string webRootPath = _webHostEnvironment.WebRootPath;
            string directoryPath = webRootPath + @"\temp\";
            string fullPath = "";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            foreach (var file in formFiles)
            {
                fullPath = @$"{directoryPath}\{file.FileName}";
                using (var fileStream = new FileStream(directoryPath + file.FileName, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    attachments.Add(fullPath);
                }
            }
#if DEBUG
#else
            _emailSender.Send("order@bath-dream.ru", $"Bath-Dream - Новая заявка {Input.Sender}",
            $"Отправитель (email/тел.): {Input.Sender}. " +
            $"Текст заявки: '{Input.Message}' ", attachments);
#endif


            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return RedirectToPage();
        }
    }
}
