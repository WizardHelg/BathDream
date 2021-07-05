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

        #region ������ ��� ������ ����������
        public List<Color> TileColors { get; init; } = new List<Color>()
        {
            new Color{
                Name = "������-����������",
                CodeBackground = "#79553d",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "���������",
                CodeBackground = "#30d5c8"
            },
            new Color{
                Name = "����������",
                CodeBackground = "#474A51",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "������������",
                CodeBackground = "#ffffff"
            },
            new Color{
                Name = "������-�������",
                CodeBackground = "#f5f5dc"
            },
            new Color{
                Name = "������-�����",
                CodeBackground = "#bbbbbb"
            },
            new Color{
                Name = "������-����������",
                CodeBackground = "#987654",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�����-�����",
                CodeBackground = "#49423d",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�����-�������",
                CodeBackground = "#ac9362",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#f5f5dc"
            },
            new Color{
                Name = "�����",
                CodeBackground = "#ffffff"
            },
            new Color{
                Name = "��������",
                CodeBackground = "#9b2d30",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#42aaff"
            },
            new Color{
                Name = "������",
                CodeBackground = "#ffff00"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#008000",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#ffd700"
            },
            new Color{
                Name = "����������",
                CodeBackground = "#964b00",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#ff0000",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "��������",
                CodeBackground = "#fdf4e3"
            },
            new Color{
                Name = "���������",
                CodeBackground = "#ffa500"
            },
            new Color{
                Name = "������������",
                CodeBackground = "#ffffff"
            },
            new Color{
                Name = "�������",
                CodeBackground = "#ffc0cb"
            },
            new Color{
                Name = "����������",
                CodeBackground = "#c0c0c0 "
            },
            new Color{
                Name = "�����",
                CodeBackground = "#808080",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "�����",
                CodeBackground = "#0000ff",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "���������",
                CodeBackground = "#c8a2c8"
            },
            new Color{
                Name = "����������",
                CodeBackground = "#8b00ff",
                CodeText = "#ffffff"
            },
            new Color{
                Name = "������",
                CodeBackground = "#000000",
                CodeText = "#ffffff"
            }
        };

        public class Color
        {
            public string Name { get; set; }
            public string CodeBackground { get; set; }
            public string CodeText { get; set; }
        }

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
                        Img = "img/brief/other/������ 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������ 1.png"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������ 3.2.jpg"
                    }
                }
            },
            ["�������������"] = new()
            {
                Name = "������� ������",
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
                        Img = "img/brief/other/������� ������ 1.png"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������� ������ 2.png"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������� ������ 3.png"
                    }
                }
            },
            ["�������������"] = new()
            {
                Name = "������� ������",
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
                        Img = "img/brief/other/������� ������ 1.png"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������� ������ 2.png"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������� ������ 3.png"
                    }
                }
            },
            ["�������"] = new()
            {
                Name = "�������",
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
                        Img = "img/brief/other/������� 1.png"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������� 2.png"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������� 3.png"
                    }
                }
            },
            ["�������������������"] = new()
            {
                Name = "������ � �������� ������������",
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
                        Img = "img/brief/other/����� � �������� ������������ 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/����� � �������� ������������ 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/����� � �������� ������������ 3.jpg"
                    }
                }
            },
            ["���������������"] = new()
            {
                Name = "������ ���������",
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
                        Img = "img/brief/other/������ 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������ 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������ 3.jpg"
                    }
                }
            },
            ["�����������"] = new()
            {
                Name = "�����������",
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
                        Img = "img/brief/other/����������� ��������� ������ 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/����������� ��������� ������ 2.jpg"
                    }
                }
            },
            ["����������������"] = new()
            {
                Name = "������������� ���",
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
                        Img = "img/brief/other/������������� ��� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������������� ��� 2.jpg"
                    }
                }
            },
            ["����"] = new()
            {
                Name = "����",
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
                        Img = "img/brief/other/���� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/���� 2.jpg"
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
                        Img = "img/brief/other/�������� 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/�������� 3.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/�������� 4.jpg"
                    }
                }
            },
            ["����������������"] = new()
            {
                Name = "���������� ������",
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
                        Img = "img/brief/other/����������������1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/����������������2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/����������������3.jpg"
                    }
                }
            },
            ["����������������"] = new()
            {
                Name = "����� ��� ��������",
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
                        Img = "img/brief/other/����� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/����� 4.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/����� ������� 1.png"
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
                        Img = "img/brief/other/�������  ��������� ��� ����� 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/�������  ��������� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������� � ���������.jpg"
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
                        Img = "img/brief/other/����������������� 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/����������������� 3.jpg"
                    }
                }
            },
            ["�����������������"] = new()
            {
                Name = "�������������� ���",
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
                        Img = "img/brief/other/�������������� ��� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/�������������� ��� 2.jpg"
                    }
                }
            },
            ["����������"] = new()
            {
                Name = "����������",
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
                        Img = "img/brief/other/������ �������������� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������ �������������� 2.jpg"
                    }
                }
            },
            ["���������������"] = new()
            {
                Name = "��������� ������",
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
                        Img = "img/brief/other/������� �����.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/������� ����� 2.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/������� �����.jpg"
                    }
                }
            },

            ///��������
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
                        Img = "img/brief/other/��������� 1.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 2",
                        Img = "img/brief/other/��������� 3.jpg"
                    },
                    new()
                    {
                        Text = "������� ����� 3",
                        Img = "img/brief/other/��������� 4.jpg"
                    }
                }
            },

            #region ����������� ��������� ����
            //["����������� ��������� ����"] = new()
            //{
            //    Name = "����������� ��������� ����",
            //    Prices = new()
            //    {
            //        "15 000 - 30 000",
            //        "30 000 - 45 000",
            //        "45 000 - 60 000",
            //        "60 000 - 75 000",
            //        "75 000 - 90 000",
            //        "90 000 - 105 000",
            //        "105 000 - ..."
            //    },
            //    ChoiseItems = new()
            //    {
            //        new()
            //        {
            //            Img = "img/brief/other/����������� ���� 1.jpg",
            //            IsRadio = false
            //        },
            //        new()
            //        {
            //            Text = "������� ����� 2",
            //            Img = "img/brief/other/����������� ���� 2.jpg",
            //        }
            //    }
            #endregion
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

        public class Style
        {
            public string Name { get; set; }
            public bool IsSelect { get; set; }
        }
        public class InputModel
        {
            public List<Style> DesignStyle { get; set; } 
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
                foreach (var work in order.Estimate.Works)
                {
                    AddSantech(work.InnerName, $"{work.Volume:F0}");
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

            for (int i = 0; i < Input.DesignStyle.Count; i++)
            {
                if (Input.DesignStyle[i].IsSelect)
                {
                    size_builder.Append($"{Input.DesignStyle[i].Name}, ");
                }
            }
            if (size_builder.Length > 0)
            {
                size_builder.Remove(size_builder.Length - 2, 2);
                builder.Append($"�����: {size_builder}<br />");
                size_builder.Clear();
            }

            builder.Append("<br />");

            builder.Append($"���� ����: {Input.FloorColor}<br />");
            builder.Append($"������ ������ ����: {Input.FloorTileBudget}<br />");
            builder.Append($"������� ������ ����: {Input.FloorLayingTilesType}<br />");
            if (Input.FloorTileSize1) size_builder.Append("���������������, ");
            if (Input.FloorTileSize2) size_builder.Append("��������������, ");
            if (Input.FloorTileSize3) size_builder.Append("� ����������� ������, ");
            if(size_builder.Length > 0)
            {
                size_builder.Remove(size_builder.Length - 2, 2);
                builder.Append($"������ ������ ����: {size_builder}<br />");
                size_builder.Clear();
            }
            builder.Append("<br />");

            builder.Append($"���� ����: {Input.WallColor}<br />");
            builder.Append($"������ ������ ����: {Input.WallTileBudget}<br />");
            builder.Append($"������� ������ ����: {Input.WallLayingTilesType}<br />");
            if (Input.WallTileSize1) size_builder.Append("���������������, ");
            if (Input.WallTileSize2) size_builder.Append("��������������, ");
            if (Input.WallTileSize3) size_builder.Append("� ����������� ������, ");
            if (size_builder.Length > 0)
            {
                size_builder.Remove(size_builder.Length - 2, 2);
                builder.Append($"������ ������ ����: {size_builder}<br />");
                size_builder.Clear();
            }
            builder.Append($"���� ������ ����: {Input.WallTileAngular}<br />");
            builder.Append("<br />");

            builder.Append($"���������: {Input.Electric}<br />");
            builder.Append("<br />");

            var styles = PageContext.HttpContext.Request.Form;
            foreach (var item in Santech)
            {
                styles.TryGetValue($"style-{item.Name}", out StringValues value);
                string style = value.Count > 0 ? value[0]?.ToString() ?? "" : null;
                builder.Append($"{item.Name} - ����������: {item.Amount}, ������: {item.Budget}");
                if (style == null)
                    builder.Append("<br />");
                else
                    builder.Append($", �����: {style}<br />");
            }

            Order submitOrder = new Order();

            if (OrderId > 0 && await _db.Orders.FirstOrDefaultAsync(o => o.Id == OrderId) is Order order)
            {
                order.Status |= Order.Statuses.Brief;
                submitOrder = order;
                await ChatHub.AddBriefMessage(builder.ToString(), user.Id, _userManager, _db, order);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("/Account/Customer");
        }
    }
}
