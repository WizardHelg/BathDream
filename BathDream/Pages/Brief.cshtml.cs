using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;
using BathDream.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IO;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Identity;
using BathDream.Services;

namespace BathDream.Pages
{
    [Authorize(Roles = "customer")]
    public class BriefModel : PageModel
    {
        readonly DBApplicationaContext _db;
        readonly EmailSender _emailSender;
        readonly UserManager<User> _userManager;

        public BriefModel(DBApplicationaContext db, EmailSender emailSender, UserManager<User> userManager)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        #region Данные для вывода сантехники
        public List<string> TileColors { get; init; } = new List<string>()
        {
            "Бежево-коричневый",
            "Бирюзовый",
            "Графитовый",
            "Многоцветный",
            "Светло-бежевый",
            "Светло-серый",
            "Светло-коричневый",
            "Темно-серый",
            "Темно-бежевый",
            "Бежевый",
            "Белый",
            "Бордовый",
            "Голубой",
            "Желтый",
            "Зеленый",
            "Золотой",
            "Коричневый",
            "Красный",
            "Кремовый",
            "Оранжевый",
            "Разноцветный",
            "Розовый",
            "Серебрянный",
            "Серый",
            "Синий",
            "Сиреневый",
            "Фиолетовый",
            "Черный"
        };

        public List<SantechItem> SantechItems { get; set; }

        readonly Dictionary<string, SantechItem> _santeh_items = new()
        {
            ["Ванная"] = new()
            {
                Name = "Ванная",
                Prices = new()
                {
                    "10 000 - 20 000",
                    "20 000 - 30 000",
                    "30 000 - 40 000",
                    "40 000 - 50 000",
                    "50 000 - 60 000",
                    "60 000 - 70 000",
                    "70 000 - 80 000",
                    "80 000 - 90 000",
                    "90 000 - 100 000",
                    "100 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Ванная 1.jpg"
                    },
                    //new()
                    //{
                    //    Text = "Вариант стиля 2",
                    //    Img = "img/brief/other/Ванная 1.png"
                    //},
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/Ванная 3.2.jpg"
                    }
                }
            },
            ["Инсталяция"] = new()
            {
                Name = "Инсталяция",
                Prices = new()
                {
                    "15 000 - 30 000",
                    "30 000 - 45 000",
                    "45 000 - 60 000",
                    "60 000 - 75 000",
                    "75 000 - 90 000",
                    "90 000 - 105 000",
                    "105 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Img = "img/brief/other/Инсталяция.jpg",
                        IsRadio = false
                    }
                }
            },
            ["Смеситель"] = new()
            {
                Name = "Смеситель",
                Prices = new()
                {
                    "1 000 - 10 000",
                    "10 000 - 20 000",
                    "20 000 - 30 000",
                    "30 000 - 40 000",
                    "40 000 - 50 000",
                    "50 000 - 60 000",
                    "60 000 - 70 000",
                    "70 000 - 80 000",
                    "80 000 - 90 000",
                    "90 000 - 100 000",
                    "100 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Смеситель 1.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Смеситель 3.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/Смеситель 4.jpg"
                    }
                }
            },
            ["Раковина"] = new()
            {
                Name = "Раковина",
                Prices = new()
                {
                    "5 000 - 10 000",
                    "10 000 - 15 000",
                    "15 000 - 20 000",
                    "20 000 - 25 000",
                    "25 000 - 30 000",
                    "30 000 - 35 000",
                    "35 000 - 40 000",
                    "40 000 - 45 000",
                    "45 000 - 50 000",
                    "50 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Раковина 2.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Раковина 3.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/Раковина 4.jpg"
                    }
                }
            },
            ["Тумба"] = new()
            {
                Name = "Тумба",
                Prices = new()
                {
                    "5 000 - 10 000",
                    "10 000 - 15 000",
                    "15 000 - 20 000",
                    "20 000 - 25 000",
                    "25 000 - 30 000",
                    "30 000 - 35 000",
                    "35 000 - 40 000",
                    "40 000 - 45 000",
                    "45 000 - 50 000",
                    "50 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Тумба 1.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Тумба 4.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/Тумба вариант 1.png"
                    }
                }
            },
            ["Зеркало"] = new()
            {
                Name = "Зеркало",
                Prices = new()
                {
                    "1 000 - 10 000",
                    "10 000 - 20 000",
                    "20 000 - 30 000",
                    "30 000 - 40 000",
                    "40 000 - 50 000",
                    "50 000 - 60 000",
                    "60 000 - 70 000",
                    "70 000 - 80 000",
                    "80 000 - 90 000",
                    "90 000 - 100 000",
                    "100 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Зеркало  Встроеное под заказ 2.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Зеркало  подвесное 1.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/зеркало с посветкой.jpg"
                    }
                }
            },
            ["Полотенцесушитель"] = new()
            {
                Name = "Полотенцесушитель",
                Prices = new()
                {
                    "5 000 - 10 000",
                    "10 000 - 15 000",
                    "15 000 - 20 000",
                    "20 000 - 25 000",
                    "25 000 - 30 000",
                    "30 000 - 35 000",
                    "35 000 - 40 000",
                    "40 000 - 45 000",
                    "45 000 - 50 000",
                    "50 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Полотенцесушитель 2.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Полотенцесушитель 3.jpg"
                    }
                }
            },
            ["Дверь"] = new()
            {
                Name = "Дверь",
                Prices = new()
                {
                    "5 000 - 10 000",
                    "10 000 - 15 000",
                    "15 000 - 20 000",
                    "20 000 - 25 000",
                    "25 000 - 30 000",
                    "30 000 - 35 000",
                    "35 000 - 40 000",
                    "40 000 - 45 000",
                    "45 000 - 50 000",
                    "50 000 - ..."
                },
                ChoiseItems = new()
                {
                    new()
                    {
                        Text = "Вариант стиля 1",
                        Img = "img/brief/other/Обычные двери.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/brief/other/Обычные двери 2.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/brief/other/Скрытая дверь.jpg"
                    }
                }
            },
        };

        public class SantechItem
        {
            public string Name { get; set; }
            public string Amount { get; set; }
            public List<string> Prices { get; set; }
            public List<ChoiseItem> ChoiseItems { get; set; }
        }

        public class ChoiseItem
        {
            public string Text { get; set; }
            public string Img { get; set; }
            public bool IsRadio { get; set; } = true;
        }
        #endregion
        #region Биндинг основных данных
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string DesignStyle { get; set; }
            public string FloorColor { get; set; }
            public string FloorTileBudget { get; set; }
            public string FloorLayingTilesType { get; set; }
            public bool FloorTileSize1 { get; set; }
            public bool FloorTileSize2 { get; set; }
            public bool FloorTileSize3 { get; set; }

            public string WallColor { get; set; }
            public string WallTileBudget { get; set; }
            public string WallLayingTilesType { get; set; }
            public bool WallTileSize1 { get; set; }
            public bool WallTileSize2 { get; set; }
            public bool WallTileSize3 { get; set; }
            public string WallTileAngular { get; set; }
            public string Electric { get; set; }
        }
        #endregion
        #region Биндинг сантехники

        [BindProperty]
        public SantechModel Santech { get; set; }
        public class SantechModel
        {
            public string[] Name { get; set; }
            public string[] Amount { get; set; }
            public string[] Budget { get; set; }

            public IEnumerator<SantechModelItem> GetEnumerator()
            {
                for (int i = 0; i < (Name?.GetLength(0) ?? 0); i++)
                    yield return new()
                    {
                        Name = Name[i],
                        Amount = Amount[i],
                        Budget = Budget[i]
                    };
            }
        }

        public class SantechModelItem
        {
            public string Name { get; init; }
            public string Amount { get; init; }
            public string Budget { get; init; }
        }
        #endregion

        [BindProperty]
        public int OrderId { get; set; }

        public async Task<IActionResult> OnGet(int id = 0)
        {
            OrderId = id;
            void AddSantech(string name, string amount)
            {
                if (_santeh_items.TryGetValue(name, out SantechItem value))
                {
                    value.Amount = amount;
                    SantechItems.Add(value);
                }
            }

            SantechItems = new();

            if (id > 0 && await _db.Orders.Include(o => o.Estimate)
                                         .ThenInclude(e => e.Works)
                                         .Where(o => o.Id == id)
                                         .FirstOrDefaultAsync() is Order order)
            {
                foreach(var work in order.Estimate.Works)
                {
                    switch (work.InnerName)
                    {
                        case "Ванная":
                            AddSantech("Ванная", $"{work.Volume:F0}");
                            AddSantech("Смеситель", $"{work.Volume:F0}");
                            break;
                        case "Инсталляция":
                            AddSantech("Инсталяция", $"{work.Volume:F0}");
                            break;
                        case "Раковина":
                            AddSantech("Раковина", $"{work.Volume:F0}");
                            break;
                        case "ТумбаПодРаковину":
                            AddSantech("Тумба", $"{work.Volume:F0}");
                            break;
                        case "Зеркало":
                            AddSantech("Зеркало", $"{work.Volume:F0}");
                            break;
                        case "Полотенцесушитель":
                            AddSantech("Полотенцесушитель", $"{work.Volume:F0}");
                            break;
                        case "УстановкаДверей":
                            AddSantech("Дверь", $"{work.Volume:F0}");
                            break;
                    }
                }

                return Page();
            }

            return RedirectToPage("/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            //await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            StringBuilder builder = new();

            StringBuilder size_builder = new();
            builder.Append($"Общий стиль: {Input.DesignStyle}<br />");
            builder.Append("<br />");

            builder.Append($"Цвет пола: {Input.FloorColor}<br />");
            builder.Append($"Бюджет плитки пола: {Input.FloorTileBudget}<br />");
            builder.Append($"Укладка плитки пола: {Input.FloorLayingTilesType}<br />");
            if (Input.FloorTileSize1) size_builder.Append("Широкоформатная, ");
            if (Input.FloorTileSize2) size_builder.Append("Мелкоформатная, ");
            if (Input.FloorTileSize3) size_builder.Append("С добавлением декора, ");
            if(size_builder.Length > 0)
            {
                size_builder.Remove(size_builder.Length - 2, 2);
                builder.Append($"Размер плитки пола: {size_builder}<br />");
                size_builder.Clear();
            }
            builder.Append("<br />");

            builder.Append($"Цвет стен: {Input.WallColor}<br />");
            builder.Append($"Бюджет плитки стен: {Input.WallTileBudget}<br />");
            builder.Append($"Укладка плитки стен: {Input.WallLayingTilesType}<br />");
            if (Input.WallTileSize1) size_builder.Append("Широкоформатная, ");
            if (Input.WallTileSize2) size_builder.Append("Мелкоформатная, ");
            if (Input.WallTileSize3) size_builder.Append("С добавлением декора, ");
            if (size_builder.Length > 0)
            {
                size_builder.Remove(size_builder.Length - 2, 2);
                builder.Append($"Размер плитки стен: {size_builder}<br />");
                size_builder.Clear();
            }
            builder.Append($"Углы плитки стен: {Input.WallTileAngular}<br />");
            builder.Append("<br />");

            builder.Append($"Электрика: {Input.Electric}<br />");
            builder.Append("<br />");

            var styles = PageContext.HttpContext.Request.Form;
            foreach (var item in Santech)
            {
                styles.TryGetValue($"style-{item.Name}", out StringValues value);
                
                string style = value[0]?.ToString() ?? "";
                builder.Append($"{item.Name} - количество: {item.Amount}, бюджет: {item.Budget}, стиль: {style}<br />");
            }

            await ChatHub.AddBriefMessage(builder.ToString(), user.Id, _userManager, _db);

            if (OrderId > 0 && await _db.Orders.FirstOrDefaultAsync(o => o.Id == OrderId) is Order order)
            {
                order.Status |= Order.Statuses.Brief;
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("/Account/Customer", "Chat");
        }
    }
}
