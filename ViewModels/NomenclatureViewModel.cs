using ReactiveUI;
using System;
using System.Collections.Generic;
using MakeOrderStu3.Models;
using MakeOrderStu3.Singletones;
using System.Text.RegularExpressions;
using NLog;
using System.Linq;
using Avalonia.Threading;

namespace MakeOrderStu3.ViewModels
{
    public class NomenclatureViewModel: ViewModelBase
    {
        #region Fields and Properties
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public override string Name { get => "Получение номенклатуры"; }

        public IObservable<bool> CanExecuteSearch { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SearchCommand { get; set; }

        public IObservable<bool> CanExecuteGetCatalog { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetCatalogCommand { get; set; }

        public IObservable<bool> CanExecuteAddNomenclature { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AddNomenclatureCommand { get; set; }

        public IObservable<bool> CanExecuteRemoveNomenclature { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> RemoveNomenclatureCommand { get; set; }

        public IObservable<bool> CanExecuteRemoveAllNomenclature { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> RemoveAllNomenclatureCommand { get; set; }

        public IObservable<bool> CanExecuteMakeOrder { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MakeOrderCommand { get; set; }

        private bool isEnabled;
        public override bool IsEnabled
        { 
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);  
        }

        private bool isWaiting;
        public bool IsWaiting
        {
            get => isWaiting;
            set => this.RaiseAndSetIfChanged(ref isWaiting, value);
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => this.RaiseAndSetIfChanged(ref searchText, value);
        }

        private List<Nomenclature> foundNomenclature;
        public List<Nomenclature> FoundNomenclature
        {
            get => foundNomenclature;
            set => this.RaiseAndSetIfChanged(ref foundNomenclature, value);
        }

        private Nomenclature selectedFoundNomenclature;
        public Nomenclature SelectedFoundNomenclature
        {
            get => selectedFoundNomenclature;
            set => this.RaiseAndSetIfChanged(ref selectedFoundNomenclature, value);            
        }

        private List<Nomenclature> addedNomenclature;
        public List<Nomenclature> AddedNomenclature
        {
            get => addedNomenclature;
            set => this.RaiseAndSetIfChanged(ref addedNomenclature, value);
        }

        private Nomenclature selectedAddedNomenclature;
        public Nomenclature SelectedAddedNomenclature
        {
            get => selectedAddedNomenclature;
            set => this.RaiseAndSetIfChanged(ref selectedAddedNomenclature, value);
        }

        private string contractCode;
        public string ContractCode
        {
            get => contractCode;
            set => this.RaiseAndSetIfChanged(ref contractCode, value);
        }

        private Order order;
        public Order Order
        {
            get => order;
            set => this.RaiseAndSetIfChanged(ref order, value);
        }
        #endregion

        #region Constructor
        public NomenclatureViewModel()
        {
            IsEnabled = true;
            if (GetSettings.Get() is null)
            {
                throw new Exception("Не прочитать настройки!");
            }
            FoundNomenclature = new List<Nomenclature>();
            AddedNomenclature = new List<Nomenclature>();
            CanExecuteGetCatalog = this.WhenAnyValue(x => x.ContractCode, (cc) => !string.IsNullOrEmpty(cc) && new Regex(@"^(C\d{9})$").IsMatch(cc));
            GetCatalogCommand = ReactiveCommand.CreateFromTask(async () => await System.Threading.Tasks.Task.Factory.StartNew(() => GetCatalog()), CanExecuteGetCatalog);
            CanExecuteSearch = this.WhenAnyValue(x => x.SearchText, (st) => !string.IsNullOrEmpty(st) && st.Trim().Length > 2);
            SearchCommand = ReactiveCommand.CreateFromTask(async () => await System.Threading.Tasks.Task.Factory.StartNew(() => Search()), CanExecuteSearch);
            CanExecuteAddNomenclature = this.WhenAnyValue(x => x.SelectedFoundNomenclature, x => x.AddedNomenclature, x => x.ContractCode, (sfn, an, cc) =>
                                            sfn != null 
                                            && !string.IsNullOrWhiteSpace(cc)
                                            && Validate.Eta(cc, sfn) 
                                            && (sfn.AllowMultipleItems || (!sfn.AllowMultipleItems && an.Count == 0 ? true : an.Where(y => y.Id == sfn.Id).Count() == 0)));
            AddNomenclatureCommand = ReactiveCommand.Create(() => { AddedNomenclature.Add(SelectedFoundNomenclature); AddedNomenclature = new List<Nomenclature>(AddedNomenclature); }, CanExecuteAddNomenclature);
            CanExecuteRemoveNomenclature = this.WhenAnyValue(x => x.SelectedAddedNomenclature, x => x.AddedNomenclature, (san, an) => san != null && an.Count > 0);
            RemoveNomenclatureCommand = ReactiveCommand.Create(() => { AddedNomenclature.Remove(SelectedAddedNomenclature); AddedNomenclature = new List<Nomenclature>(AddedNomenclature); }, CanExecuteRemoveNomenclature);
            CanExecuteRemoveAllNomenclature = this.WhenAnyValue(x => x.AddedNomenclature, (an) => an != null && an.Count > 0);
            RemoveAllNomenclatureCommand = ReactiveCommand.Create(() => { AddedNomenclature = new List<Nomenclature>(); }, CanExecuteRemoveAllNomenclature);
            CanExecuteMakeOrder = this.WhenAnyValue(x => x.AddedNomenclature, (an) => an != null && an.Count > 0);
            MakeOrderCommand = ReactiveCommand.Create(() => MakeOrder(), CanExecuteMakeOrder);
        }
        #endregion

        #region Methods
        private void GetCatalog()
        {
            IsWaiting = true;
            logger.Debug("Получаем номенклатуру!");
            try
            {
                NomenclatureList.Get(ContractCode);
                logger.Debug("Номенклатура получена!");
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.InvokeAsync(() => SearchText = "Не удалось получить номенклатуру!!!");
                logger.Error($"Не удалось получить номенклатуру!!!\r\n{ex}");
            }
            finally
            {
                IsWaiting = false;
            }
        }

        private void Search()
        {
            try 
            {
                FoundNomenclature = NomenclatureList.Get(ContractCode)?.Where(x =>
                                        x.Id.ToUpper().Contains(SearchText.ToUpper()) ||
                                        x.LabId.ToUpper().Contains(SearchText.ToUpper()) ||
                                        x.Caption.ToUpper().Contains(SearchText.ToUpper()) ||
                                        x.Description.ToUpper().Contains(SearchText.ToUpper())).ToList();
            }
            catch (Exception ex)
            {
                SearchText = "Не удалось получить номенклатуру!!!";
                logger.Error($"Не удалось получить номенклатуру!!!\r\n{ex}");
            }            
        }
        private void MakeOrder()
        {
            try
            {
                Order = GetOrder.Get(AddedNomenclature, ContractCode);
            }
            catch (Exception ex)
            {
                SearchText = "Заказ не создан!!!";
                logger.Error($"Заказ не создан!!!\r\n{ex}");
            }
        }
        #endregion
    }
}
