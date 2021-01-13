using MakeOrderStu3.Models;
using MakeOrderStu3.Models.PreanalyticsClasses;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using ReactiveUI;
using System;

namespace MakeOrderStu3.ViewModels
{
    public class PreanalyticsViewModel: ViewModelBase
    {
        #region Fields and properties
        public override string Name
        {
            get => "Преаналитика";
        }
        public IObservable<bool> canExecuteGetPreanalytics { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetPreanalyticsCommand { get; set; }

        private bool isEnabled;
        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref isEnabled, value);
                if (value)
                {
                    BodyForPreanalytics = JsonConvert.SerializeObject(GetOrder.Get().PreanalyticsRequest, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    if (BodyForPreanalytics.Length > 20)
                    {
                        GetPreanalyticsCommand.Execute();
                    }
                }
            }
        }

        private bool isWaiting;
        public bool IsWaiting
        {
            get => isWaiting;
            set => this.RaiseAndSetIfChanged(ref isWaiting, value);
        }

        private string bodyForPreanalytics;
        public string BodyForPreanalytics
        {
            get => bodyForPreanalytics;
            set => this.RaiseAndSetIfChanged(ref bodyForPreanalytics, value);
        }

        private string preanalyticsResult;
        public string PreanalyticsResult
        {
            get => preanalyticsResult;
            set => this.RaiseAndSetIfChanged(ref preanalyticsResult, value);
        }
        #endregion
        #region .ctor
        public PreanalyticsViewModel()
        {
            canExecuteGetPreanalytics = this.WhenAnyValue(x => x.BodyForPreanalytics, (bfp) => bfp != null && bfp.Length > 0);
            GetPreanalyticsCommand = ReactiveCommand.Create(() => StartCommandGetPreanalytics(), canExecuteGetPreanalytics);
        }
        #endregion
        #region Methods

        public async void StartCommandGetPreanalytics()
        {            
            PreanalyticsResult = string.Empty;
            PreanalyticsResult = await System.Threading.Tasks.Task.Factory.StartNew(() => GetPreanalytics());
            try
            {
                GetOrder.AddResource(JsonConvert.DeserializeObject<AnalyticsResponse>(PreanalyticsResult));
            }
            catch (Exception ex)
            {
                PreanalyticsResult += $"\r\n\r\nНе удалось добавить ответ преаналитики в заказ по причине:\r\n{ex}";
            }
        }

        private string GetPreanalytics()
        {
            string result;
            IsWaiting = true;
            try
            {
                result = GetFromService.PreanaliticsResult(BodyForPreanalytics);
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            finally
            {
                IsWaiting = false;
            }
            return result;
        }
        #endregion

    }
}
