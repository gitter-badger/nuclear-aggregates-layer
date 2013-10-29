using System;
using System.Collections.Generic;

using DoubleGis.Erm.Core.Utils;

namespace DoubleGis.Erm.Core.Dto.DomainEntity.Custom
{
    public static class DebugDataProvider
    {
        public static OrderPositionDto GetOrderPositionDto()
        {
            return new OrderPositionDto
                {
                    Id = -1,
                    PricePosition = "Условный пакет",
                    Platform = "Неопределённая платформа",
                    Price = new OrderPositionPriceDto(),
                    Advertisements = GetAdvertisements(),
                };
        }

        public static IEnumerable<OrderPositionAdvertisementDto> GetAdvertisements()
        {
            return new List<OrderPositionAdvertisementDto>
                {
                    new OrderPositionAdvertisementDto
                        {
                            Type = LinkingObjectType.PricePosition,
                            Name = "Абонентская плата Mobile",
                            CanBeChecked = true,
                            CanBeLinked = false,
                        },

                    new OrderPositionAdvertisementDto
                        {
                            Type = LinkingObjectType.PricePosition,
                            Name = "Выгодные покупки с 2ГИС к адресу",
                            CanBeChecked = false,
                            CanBeLinked = false,

                            Children = new List<OrderPositionAdvertisementDto>
                                {
                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Address,
                                            Name = "Тюмень, Народная, 16",
                                            CanBeChecked = true,
                                            CanBeLinked = true,
                                        },

                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Address,
                                            Name = "Lemesos (Limassol обл.), Αγίας Σοφίας, 101d-101g — ",
                                            CanBeChecked = true,
                                            CanBeLinked = true,
                                        },
                                },
                        },

                    new OrderPositionAdvertisementDto
                        {
                            Type = LinkingObjectType.PricePosition,
                            Name = "Баннер в рубрике",
                            CanBeChecked = false,
                            CanBeLinked = false,
                            Children = new List<OrderPositionAdvertisementDto>
                                {
                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Category,
                                            Name = "Заборы / Ограждения",
                                            CanBeChecked = true,
                                            CanBeLinked = true,
                                        },

                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Category,
                                            Name = "Металлоизделия",
                                            CanBeChecked = true,
                                            CanBeLinked = true,
                                        },

                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Category,
                                            Name = "Интерьерные лестницы / Ограждения",
                                            CanBeChecked = true,
                                            CanBeLinked = true,

                                            IsChecked = true,
                                            AdvertisementLink = new EntityReference(1, "банн"),
                                        },

                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Category,
                                            Name = "Садово-парковая мебель / Аксессуары",
                                            CanBeChecked = true,
                                            CanBeLinked = true,
                                        },
                                },
                        },

                    new OrderPositionAdvertisementDto
                        {
                            Type = LinkingObjectType.PricePosition,
                            Name = "Логотип на карте во всех масштабах в одной рубрике",
                            CanBeChecked = false,
                            CanBeLinked = false,
                            Children = new List<OrderPositionAdvertisementDto>
                                {
                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Address,
                                            Name = "Тюмень, Народная, 16",
                                            CanBeChecked = false,
                                            CanBeLinked = false,
                                            Children = new List<OrderPositionAdvertisementDto>
                                                {
                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Заборы / Ограждения",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Металлоизделия",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Интерьерные лестницы / Ограждения",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Садово-перковая мебель / Аксессуары",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },
                                                },
                                        },

                                    new OrderPositionAdvertisementDto
                                        {
                                            Type = LinkingObjectType.Address,
                                            Name = "Lemesos (Limassol обл.), Αγίας Σοφίας, 101d-101g — ",
                                            CanBeChecked = false,
                                            CanBeLinked = false,
                                            Children = new List<OrderPositionAdvertisementDto>
                                                {
                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Заборы / Ограждения",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Металлоизделия",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Интерьерные лестницы / Ограждения",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },

                                                    new OrderPositionAdvertisementDto
                                                        {
                                                            Type = LinkingObjectType.Category,
                                                            Name = "Садово-перковая мебель / Аксессуары",
                                                            CanBeChecked = true,
                                                            CanBeLinked = true,
                                                        },
                                                },
                                        },
                                },
                        },
                };
        }

        public static OrderPositionAdvertisementDto GetRandomCategory()
        {
            var names = new[]
                {
                    "Услуги телефонной связи",
                    "Мэрия города Омска",
                    "Иконописные мастерские",
                    "Аудиореклама в магазинах от РК \"Любимый город\"",
                    "Правительство Архангельской области",
                    "Наружная реклама РГ \"Армада Аутдор\"",
                    "Коррекция зрения / Лечение офтальмологических заболеваний",
                    "Материалы и оборудование для монолитного строительства от \"ТСС Групп\"",
                    "Продажа автобусов",
                    "Телекомпания \"Волга\"",
                    "Администрация кондоминиума",
                    "Холодильное оборудование",
                    "Свадебные товары",
                    "Администрация города Волгограда",
                    "Создание субтитров"
                };

            var name = names[new Random().Next(0, names.Length)];
            return new OrderPositionAdvertisementDto
            {
                Type = LinkingObjectType.Category,
                Name = name,
                CanBeChecked = true,
                CanBeLinked = true,
            };
        }
    }
}
