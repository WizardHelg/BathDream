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

        #region ������ ��� ������ ����������
        public List<string> TileColors { get; init; } = new List<string>()
        {
            "������-����������",
            "���������",
            "����������",
            "������������",
            "������-�������",
            "������-�����",
            "������-����������",
            "�����-�����",
            "�����-�������",
            "�������",
            "�����",
            "��������",
            "�������",
            "������",
            "�������",
            "�������",
            "����������",
            "�������",
            "��������",
            "���������",
            "������������",
            "�������",
            "�����������",
            "�����",
            "�����",
            "���������",
            "����������",
            "������"
        };

        public List<SantechItem> SantechItems { get; set; }

        readonly Dictionary<string, SantechItem> _santeh_items = new()
        {
            ["������"] = new()
            {
                Name = "������",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["����������"] = new()
            {
                Name = "����������",
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
            ["���������"] = new()
            {
                Name = "���������",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["��������"] = new()
            {
                Name = "��������",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["�����"] = new()
            {
                Name = "�����",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["�������"] = new()
            {
                Name = "�������",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["�����������������"] = new()
            {
                Name = "�����������������",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/Bath.jpg"
                    }
                }
            },
            ["�����"] = new()
            {
                Name = "�����",
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
                        Text = "������� ����� 1",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/Bath.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
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
        #region ������� �������� ������
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
        #region ������� ����������

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
                        case "������":
                            AddSantech("������", $"{work.Volume:F0}");
                            AddSantech("���������", $"{work.Volume:F0}");
                            break;
                        case "�����������":
                            AddSantech("����������", $"{work.Volume:F0}");
                            break;
                        case "��������":
                            AddSantech("��������", $"{work.Volume:F0}");
                            break;
                        case "����������������":
                            AddSantech("�����", $"{work.Volume:F0}");
                            break;
                        case "�������":
                            AddSantech("�������", $"{work.Volume:F0}");
                            break;
                        case "�����������������":
                            AddSantech("�����������������", $"{work.Volume:F0}");
                            break;
                        case "���������������":
                            AddSantech("�����", $"{work.Volume:F0}");
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
            builder.AppendLine($"����� �����: {Input.DesignStyle}");
            builder.AppendLine();

            builder.AppendLine($"���� ����: {Input.FloorColor}");
            builder.AppendLine($"������ ������ ����: {Input.FloorTileBudget}");
            builder.AppendLine($"������� ������ ����: {Input.FloorLayingTilesType}");
            builder.AppendLine($"������ ������ ����: {Input.FloorTileSize}");
            builder.AppendLine();

            builder.AppendLine($"���� ����: {Input.WallColor}");
            builder.AppendLine($"������ ������ ����: {Input.WallTileBudget}");
            builder.AppendLine($"������� ������ ����: {Input.WallLayingTilesType}");
            builder.AppendLine($"������ ������ ����: {Input.WallTileSize}");
            builder.AppendLine($"���� ������ ����: {Input.WallTileAngular}");
            builder.AppendLine();

            var styles = PageContext.HttpContext.Request.Form;
            foreach (var item in Santech)
            {
                styles.TryGetValue($"style-{item.Name}", out StringValues value);
                
                string style = value[0]?.ToString() ?? "";
                builder.AppendLine($"{item.Name} - ����������: {item.Amount}, ������: {item.Budget}, �����: {style}");
            }

            return Content(builder.ToString());
        }
    }
}
