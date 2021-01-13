using MakeOrderStu3.Models;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MakeOrderStu3.ViewModels
{
    class SpecimenChoiceViewModel: ViewModelBase
    {
        #region Fields and Properties
        public override string Name
        {
            get => "Выбор образцов";
        }
        public IObservable<bool> canExecuteSelectSpecimens { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SelectSpecimensCommand { get; set; }
        public IObservable<bool> canExecuteCreateRequestForPreanalytics { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CreateRequestForPreanalyticsCommand { get; set; }

        private bool isEnabled;
        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref isEnabled, value);
                if (value)
                {
                    OrderText = GetOrder.Get().ToString();
                    SelectSpecimensCommand.Execute();
                }
            }
        }

        private string orderText;
        public string OrderText
        {
            get => orderText;
            set => this.RaiseAndSetIfChanged(ref orderText, value);
        }

        private List<Nomenclature> singleChoiceSpecimenPositionsList;
        public List<Nomenclature> SingleChoiceSpecimenPositionsList
        {
            get => singleChoiceSpecimenPositionsList;
            set => this.RaiseAndSetIfChanged(ref singleChoiceSpecimenPositionsList, value);
        }

        private List<Nomenclature> multipleChoiceSpecimenPositionsList;
        public List<Nomenclature> MultipleChoiceSpecimenPositionsList
        {
            get => multipleChoiceSpecimenPositionsList;
            set => this.RaiseAndSetIfChanged(ref multipleChoiceSpecimenPositionsList, value);
        }
        #endregion
        #region .ctor
        public SpecimenChoiceViewModel()
        {
            canExecuteSelectSpecimens = this.WhenAnyValue(x => x.IsEnabled, x => x.OrderText, (ie, ot) => ie == true && ot != null);
            SelectSpecimensCommand = ReactiveCommand.CreateFromTask(async () => await System.Threading.Tasks.Task.Factory.StartNew(() => SelectSpecimens()), canExecuteSelectSpecimens);
            canExecuteCreateRequestForPreanalytics = this.WhenAnyValue(x => x.OrderText, (ot) => !string.IsNullOrWhiteSpace(ot));
            CreateRequestForPreanalyticsCommand = ReactiveCommand.CreateFromTask(async () => await System.Threading.Tasks.Task.Factory.StartNew(() => CreateRequestForPreanalytics()), canExecuteCreateRequestForPreanalytics);
        }
        #endregion
        #region Methods
        public void SelectSpecimens()
        {
            SingleChoiceSpecimenPositionsList = GetOrder.Get()?.OrderPositions?.Where(x => !x.NomenclaturePosition.MultipleSpecimen).Select(y => y.NomenclaturePosition).ToList();
            MultipleChoiceSpecimenPositionsList = GetOrder.Get()?.OrderPositions?.Where(x => x.NomenclaturePosition.MultipleSpecimen).Select(y => y.NomenclaturePosition).ToList();
        }

        public void CreateRequestForPreanalytics()
        {
            List<SpecimenNomenclature> selectedSpecimens = new List<SpecimenNomenclature>();
            foreach (var pos in SingleChoiceSpecimenPositionsList)
            {
                OrderText += "\r\nSingleChoiceSpecimenPositionsList count" + pos.SelectedSpecimens.Count;
                if (pos.SelectedSpecimens.Count > 0)
                {
                    selectedSpecimens.AddRange(pos.SelectedSpecimens);                    
                }
            }
            foreach (var pos in MultipleChoiceSpecimenPositionsList)
            {
                OrderText += "\r\nMultipleChoiceSpecimenPositionsList count" + pos.SelectedSpecimens.Count;
                if (pos.SelectedSpecimens.Count > 0)
                {
                    selectedSpecimens.AddRange(pos.SelectedSpecimens);
                }

            }
            GetOrder.Get().SelectedSpecimens = selectedSpecimens;
            OrderText += "\r\n" + selectedSpecimens.Count;
            OrderText += "\r\n" + GetOrder.Get().SelectedSpecimens.Count;
            GetOrder.AddResource(Make.PreanalyticsRequest());
            OrderText += "\r\n" + JsonConvert.SerializeObject(GetOrder.Get().PreanalyticsRequest, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
        #endregion
    }
}