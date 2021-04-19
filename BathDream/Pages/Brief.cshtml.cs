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

namespace BathDream.Pages
{
    //[Authorize]
    public class BriefModel : PageModel
    {
        readonly DBApplicationaContext _db;

        public BriefModel(DBApplicationaContext db) => _db = db;

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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg",
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "Вариант стиля 3",
                        Img = "img/Bath.jpg"
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
            public string FloorTileSize { get; set; }

            public string WallColor { get; set; }
            public string WallTileBudget { get; set; }
            public string WallLayingTilesType { get; set; }
            public string WallTileSize { get; set; }
            public string WallTileAngular { get; set; }
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
                for (int i = 0; i < Name.GetLength(0); i++)
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

        public async Task<IActionResult> OnGet(int id = 0)
        {
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

        public IActionResult OnPost()
        {
            StringBuilder builder = new();
            builder.AppendLine($"Общий стиль: {Input.DesignStyle}");
            builder.AppendLine();

            builder.AppendLine($"Цвет пола: {Input.FloorColor}");
            builder.AppendLine($"Бюджет плитки пола: {Input.FloorTileBudget}");
            builder.AppendLine($"Укладка плитки пола: {Input.FloorLayingTilesType}");
            builder.AppendLine($"Размер плитки пола: {Input.FloorTileSize}");
            builder.AppendLine();

            builder.AppendLine($"Цвет стен: {Input.WallColor}");
            builder.AppendLine($"Бюджет плитки стен: {Input.WallTileBudget}");
            builder.AppendLine($"Укладка плитки стен: {Input.WallLayingTilesType}");
            builder.AppendLine($"Размер плитки стен: {Input.WallTileSize}");
            builder.AppendLine($"Углы плитки стен: {Input.WallTileAngular}");
            builder.AppendLine();

            var styles = PageContext.HttpContext.Request.Form;
            foreach (var item in Santech)
            {
                styles.TryGetValue($"style-{item.Name}", out StringValues value);
                
                string style = value[0]?.ToString() ?? "";
                builder.AppendLine($"{item.Name} - количество: {item.Amount}, бюджет: {item.Budget}, стиль: {style}");
            }

            return Content(builder.ToString());
        }
    }
}
