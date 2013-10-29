﻿namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public static class EFUtils
    {
        /// <summary>
        /// Конвертирует параметры сохранения из ORM независимого типа ERM в 
        /// формат специфичный для EF
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static System.Data.Objects.SaveOptions ToEFSaveOptions(this SaveOptions options)
        {
            return (System.Data.Objects.SaveOptions)(int)options;
        }
    }
}
