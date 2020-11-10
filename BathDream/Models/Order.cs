using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Order
    {
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
}
