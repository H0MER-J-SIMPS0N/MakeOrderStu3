using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MakeOrderStu3.Models;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;
using System;

namespace MakeOrderStu3.ViewModels
{
    public class AssembleWholeOrderViewModel: ViewModelBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public override string Name { get => "Собрать заказ"; }
        public IObservable<bool> canExecuteCreateResultBundle { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CreateResultBundleCommand { get; set; }
        public IObservable<bool> canExecuteSendResultBundle { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SendResultBundleCommand { get; set; }

        private bool isEnabled;
        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref isEnabled, value);
                logger.Debug($"Собрать заказ isEnabled {value}");
                if (value)
                {
                    NomenclatureListInOrder = GetOrder.Get()?.ToString();
                    PreanalyticsResult = GetOrder.Get()?.AnalyticsResponse?.ToString();
                    QuestionnaireResult = JsonConvert.SerializeObject(GetOrder.Get().GroupResultItems, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    logger.Debug($"NomenclatureListInOrder length {NomenclatureListInOrder?.Length}");
                    logger.Debug($"PreanalyticsResult length {PreanalyticsResult?.Length}");
                    if (NomenclatureListInOrder?.Trim().Length > 20 && PreanalyticsResult?.Trim().Length > 20)
                    {
                        CreateResultBundleCommand.Execute();
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

        private string nomenclatureListInOrder;
        public string NomenclatureListInOrder
        {
            get => nomenclatureListInOrder;
            set => this.RaiseAndSetIfChanged(ref nomenclatureListInOrder, value);
        }

        private string preanalyticsResult;
        public string PreanalyticsResult
        {
            get => preanalyticsResult;
            set => this.RaiseAndSetIfChanged(ref preanalyticsResult, value);
        }

        private string questionnaireResult;
        public string QuestionnaireResult
        {
            get => questionnaireResult;
            set => this.RaiseAndSetIfChanged(ref questionnaireResult, value);
        }

        private string bundleToSend;
        public string BundleToSend
        {
            get => bundleToSend;
            set => this.RaiseAndSetIfChanged(ref bundleToSend, value);
        }

        public AssembleWholeOrderViewModel()
        {
            canExecuteCreateResultBundle = this.WhenAnyValue(x => x.PreanalyticsResult, x => x.NomenclatureListInOrder, (pr, nlio) => !string.IsNullOrEmpty(pr) && !string.IsNullOrEmpty(nlio));
            CreateResultBundleCommand = ReactiveCommand.Create(() => StartCommandCreateResultBundle(), canExecuteCreateResultBundle);
            canExecuteSendResultBundle = this.WhenAnyValue(x => x.BundleToSend, (bts) => !string.IsNullOrEmpty(bts));
            SendResultBundleCommand = ReactiveCommand.Create(() => StartCommandSendResultBundle(), canExecuteSendResultBundle);
        }


        public async void StartCommandCreateResultBundle()
        {
            BundleToSend = string.Empty;
            BundleToSend = await System.Threading.Tasks.Task.Factory.StartNew(() => CreateResultBundle());
        }

        public string CreateResultBundle()
        {
            string result;
            IsWaiting = true;
            try
            {
                result = Make.ResultBundle(NomenclatureListInOrder, PreanalyticsResult, QuestionnaireResult);
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

        public async void StartCommandSendResultBundle()
        {
            SerializerSettings sett = new SerializerSettings()
            {
                Pretty = true
            };
            var serializer = new FhirJsonSerializer(sett);
            var parser = new FhirJsonParser();
            BundleToSend = serializer.SerializeToString(parser.Parse<Bundle>(await System.Threading.Tasks.Task.Factory.StartNew(() => SendResultBundle())));
        }

        public string SendResultBundle()
        {
            string result;
            IsWaiting = true;
            try
            {
                result = GetFromService.ResultBundle(BundleToSend);
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

    }
}
