using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using System.Globalization;

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
        //public bool CombinedBathroom { get; set; }

        /// <summary>
        /// Требуется демонтаж?
        /// </summary>
        public bool RequiredRemoval { get; set; }

        /// <summary>
        /// Требуется ли замена труб
        /// </summary>
        public bool RequiredReplacePipeline { get; set; }

        /// <summary>
        /// Количество розеток и выключателей
        /// </summary>
        public int SwitchAmount { get; set; }

        /// <summary>
        /// Теплый пол
        /// </summary>
        public bool WarmFloor { get; set; }

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

        /// <summary>
        /// Сантехнический люк
        /// </summary>
        public int PlumbingHatch { get; set; }
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
        #region PipelineEquipment
        /// <summary>
        /// Кран перекрытия
        /// </summary>
        public int PLOverheadCrane { get; set; }

        /// <summary>
        /// Фильр грубой очистки
        /// </summary>
        public int PLCoarseFilter { get; set; }

        /// <summary>
        /// Обратный клапан
        /// </summary>
        public int PLCheckValve { get; set; }

        /// <summary>
        /// Счётчик
        /// </summary>
        public int PLCounter { get; set; }

        /// <summary>
        /// Водяной коллектор ХВ
        /// </summary>
        public int PLWaterCollectorCW { get; set; }

        /// <summary>
        /// Водяной коллектор ГВ
        /// </summary>
        public int PLWaterCollectorHW { get; set; }
        #endregion
    }
}
