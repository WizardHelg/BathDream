using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;

namespace BathDream.Models
{
    public class Order
    {
        #region BathRoomParametres
        /// <summary>
        /// Тип объекта
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Санузен совмещенный?
        /// </summary>
        public bool CombinedBathroom { get; set; }

        /// <summary>
        /// Требуется демонтаж?
        /// </summary>
        public bool RequiredRemoval { get; set; }

        /// <summary>
        /// Теплый пол
        /// </summary>
        public bool WarmFloor { get; set; }

        /// <summary>
        /// Количество розеток и выключателей
        /// </summary>
        public int SwitchAmount { get; set; }

        /// <summary>
        /// Полы
        /// </summary>
        public string FloorType { get; set; }

        /// <summary>
        /// Требуется возведение стен?
        /// </summary>
        public bool NeedMakeWalls { get; set; }

        /// <summary>
        /// Стены
        /// </summary>
        public string WallCoverType { get; set; }

        /// <summary>
        /// Потолок
        /// </summary>
        public string CeilingCoverType { get; set; }

        /// <summary>
        /// Установка двери
        /// </summary>
        public bool InstallDoor { get; set; }
        #endregion
        #region BathroomEquipments
        /// <summary>
        /// Ванная
        /// </summary>
        public int BathAmount { get; set; }

        /// <summary>
        /// Душевая кабина
        /// </summary>
        public int ShowerAmount { get; set; }

        /// <summary>
        /// Душевой уголок
        /// </summary>
        public int ShowerConerAmount { get; set; }

        /// <summary>
        /// Джакузи
        /// </summary>
        public int JacuzziAmount { get; set; }

        /// <summary>
        /// Ванна с функцией гидромассажа
        /// </summary>
        public int HydroBathAmount { get; set; }

        /// <summary>
        /// Унитаз
        /// </summary>
        public int ToiletAmount { get; set; }

        /// <summary>
        /// Инсталляция + подвесной унитаз
        /// </summary>
        public int InstallationAndToiletAmount { get; set; }

        /// <summary>
        /// Инсталляция биде + подвесное биде
        /// </summary>
        public int InstallationAndBidetAmount { get; set; }

        /// <summary>
        /// Гигиенический душ
        /// </summary>
        public int HygienicShowerAmount { get; set; }

        /// <summary>
        /// Биде
        /// </summary>
        public int BidetAmount { get; set; }

        /// <summary>
        /// Раковина
        /// </summary>
        public int SinkAmount { get; set; }

        /// <summary>
        /// Тумба
        /// </summary>
        public int BedsideAmount { get; set; }

        /// <summary>
        /// Зеркало
        /// </summary>
        public int MirrorAmount { get; set; }

        /// <summary>
        /// Полотенцесушитель
        /// </summary>
        public int TowelDryerAmount { get; set; }

        /// <summary>
        /// Ванные принадлежности
        /// </summary>
        public int BathroomAccessoriesAmount { get; set; }
        #endregion
    }

    public class Rooms
    {
        [BindProperty(Name = "room_name")]
        public string[] Names { get; set; }

        [BindProperty(Name = "room_length")]
        public string[] Lengths { get; set; }

        [BindProperty(Name = "room_width")]
        public string[] Widths { get; set; }

        [BindProperty(Name = "room_height")]
        public string[] Heights { get; set; }

        [BindProperty(Name = "door_width")]
        public string[] DoorWidths { get; set; }

        [BindProperty(Name = "door_height")]
        public string[] DoorHeights { get; set; }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Names.GetLength(0); i++)
                yield return new Room
                {
                    Name = Names[i],
                    Width = double.TryParse(Widths[i], out double resW) ? resW : 0.0,
                    Height = double.TryParse(Heights[i], out double resH) ? resH : 0.0,
                    Length = double.TryParse(Lengths[i], out double resL) ? resL : 0.0,
                    Door = new Door()
                    {
                        Width = double.TryParse(DoorWidths[i], out double resDW) ? resDW : 0.0,
                        Height = double.TryParse(DoorHeights[i], out double resDH) ? resDH : 0.0
                    }
                };
        }

        public Room this[int i] => new Room
        {
            Name = Names[i],
            Width = double.TryParse(Widths[i], out double resW) ? resW : 0.0,
            Height = double.TryParse(Heights[i], out double resH) ? resH : 0.0,
            Length = double.TryParse(Heights[i], out double resL) ? resL : 0.0,
            Door = new Door()
            {
                Width = double.TryParse(DoorWidths[i], out double resDW) ? resDW : 0.0,
                Height = double.TryParse(DoorHeights[i], out double resDH) ? resDH : 0.0
            }
        };
    }

    public struct Room
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public Door Door { get; set; }

        public double FloorArea() => Width * Length;
        public double CeilingArea() => Width * Length;
        public double WallsArea() => 2 * (Width * Length) * Height - Door.Area();
    }

    public struct Door
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double Area() => Width * Height;
    }
}
