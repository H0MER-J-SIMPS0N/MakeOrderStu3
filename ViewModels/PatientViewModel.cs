using Hl7.Fhir.Model;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using ReactiveUI;
using System;

namespace MakeOrderStu3.ViewModels
{
    public sealed class PatientViewModel: ViewModelBase
    {
        #region Fields and properties
        public override string Name
        {
            get => "Пациент";
        }
        public IObservable<bool> canExecuteAddToOrder { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AddToOrderCommand { get; set; }

        private bool isEnabled;
        public override bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);            
        }

        private string patient;
        public string Patient
        {
            get => patient;
            set => this.RaiseAndSetIfChanged(ref patient, value);
        }
        #endregion
        #region .ctor
        public PatientViewModel()
        {
            Patient = @"https://api-stage.medlinx.online/fhir/Patient/c24e48e2-a303-45c9-bfab-860a5a545677";
            canExecuteAddToOrder = this.WhenAnyValue(x => x.Patient, (p) => !string.IsNullOrWhiteSpace(p) );
            AddToOrderCommand = ReactiveCommand.Create(() => AddToOrder(), canExecuteAddToOrder);
            
            //Patient = "{\r\n    \"resourceType\": \"Patient\",\r\n    \"name\": [{\r\n            \"family\": \"Test\",\r\n            \"given\": [\"Test\", \"Test\"]\r\n        }\r\n    ],\r\n    \"telecom\": [{\r\n            \"system\": \"phone\",\r\n            \"value\": \"7000000000\",\r\n            \"use\": \"mobile\"\r\n        }, {\r\n            \"system\": \"email\",\r\n            \"value\": \"-\"\r\n        }\r\n    ],\r\n    \"gender\": \"male\",\r\n    \"birthDate\": \"" + DateTime.Now.AddYears(-35).ToString("yyyy-MM-dd") + "\", \r\n    \"meta\": {\r\n        \"security\": [{\r\n                \"system\": \"read\",\r\n                \"code\": \"service\"\r\n            }\r\n        ]\r\n    }\r\n}";
        }
        #endregion
        #region Methods
        private void AddToOrder()
        {
            try
            {
                if (Patient.StartsWith(@"https://api.medlinx.online/fhir/Patient/", StringComparison.OrdinalIgnoreCase) || Patient.StartsWith(@"https://api-stage.medlinx.online/fhir/Patient/", StringComparison.OrdinalIgnoreCase))
                {
                    GetOrder.Get().PatientUrl = Patient;
                }
                else
                {
                    GetOrder.AddResource(JsonConvert.DeserializeObject<Patient>(Patient, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));                    
                }                
                Patient = GetOrder.Get().PatientUrl;
            }
            catch (Exception ex)
            {
                Patient += $"\r\n\r\nНе удалось добавить пациента в заказ по причине:\r\n{ex}";
            }
        }
        #endregion

    }
}
