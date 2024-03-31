using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Infrastructure.Data
{
    public static class DataConstants
    {
        public const int BookTitleMinLength = 3;
        public const int BookTitleMaxLength = 20;

        public const int BookDescriptionMinLength = 20;
        public const int BookDescriptionMaxLength = 500;

        public const int PenColorMinLength = 3;
        public const int PenColorMaxLength = 10;

        public const int PenManufacturerMinLength = 10;
        public const int PenManufacturerMaxLength = 40;

        public const double PenMinInkCapacity = 20;
        public const double PenMaxInkCapacity = 70;

        public const int PaperSizeMinLength = 2;
        public const int PaperSizeMaxLength = 2;

        public const int PaperColorMinLength = 3;
        public const int PaperColorMaxLength = 10;

        public const int PaperManufacturerMinLength = 10;
        public const int PaperManufacturerMaxLength = 40;
    }
}
