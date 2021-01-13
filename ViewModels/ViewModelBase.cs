using System;
using System.Collections.Generic;
using System.Text;
using MakeOrderStu3.Models;
using ReactiveUI;

namespace MakeOrderStu3.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public virtual string Name { get; set; }
        public virtual bool IsEnabled { get; set; }
    }
}
