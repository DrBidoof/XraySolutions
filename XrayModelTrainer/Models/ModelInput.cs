using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrayModelTrainer.Models
{
    internal class ModelInput
    {
        [LoadColumn(0)]
        public string ImagePath { get; set; }
    }
}
