using MakeOrderStu3.Models;
using MakeOrderStu3.Models.QuestionnaireClasses;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakeOrderStu3.ViewModels
{
    public class QuestionnaireViewModel: ViewModelBase
    {
        #region Fields and properties
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public override string Name
        {
            get => "Опросник";
        }

        private bool isEnabled;
        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref isEnabled, value);
                if (isEnabled)
                {
                    BodyForPreanalytics = JsonConvert.SerializeObject(GetOrder.Get().PreanalyticsRequest, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    logger.Debug($"BodyForPreanalytics length {BodyForPreanalytics.Length}");
                    if (BodyForPreanalytics?.Trim().Length > 20)
                    {
                        GetQuestionsCommand.Execute();
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

        private List<QuestionnaireAnswer> choiceItems;
        public List<QuestionnaireAnswer> ChoiceItems
        {
            get => choiceItems;
            set => this.RaiseAndSetIfChanged(ref choiceItems, value);
        }

        private List<QuestionnaireAnswer> stringItems;
        public List<QuestionnaireAnswer> StringItems
        {
            get => stringItems;
            set => this.RaiseAndSetIfChanged(ref stringItems, value);
        }
        public IObservable<bool> canExecuteGetQuestions { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GetQuestionsCommand { get; set; }
        public IObservable<bool> canExecuteMakeQuestionnaireResponse { get; set; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> MakeQuestionnaireResponseCommand { get; set; }

        private string bodyForPreanalytics;
        public string BodyForPreanalytics
        {
            get => bodyForPreanalytics;
            set => this.RaiseAndSetIfChanged(ref bodyForPreanalytics, value);
        }

        private string questionnaireResult;
        public string QuestionnaireResult
        {
            get => questionnaireResult;
            set => this.RaiseAndSetIfChanged(ref questionnaireResult, value);
        }

        #endregion
        #region .ctor
        public QuestionnaireViewModel()
        {
            ChoiceItems = new List<QuestionnaireAnswer>();
            StringItems = new List<QuestionnaireAnswer>();
            canExecuteGetQuestions = this.WhenAnyValue(x => x.BodyForPreanalytics, (bfp) => string.IsNullOrWhiteSpace(bfp) || bfp.Length > 0);
            GetQuestionsCommand = ReactiveCommand.Create(() => StartCommandGetQuestions(), canExecuteGetQuestions);
            canExecuteMakeQuestionnaireResponse = this.WhenAnyValue(x => x.QuestionnaireResult, x => x.ChoiceItems, x => x.StringItems, (qr, ci, si) =>
                                    !string.IsNullOrWhiteSpace(qr));
            MakeQuestionnaireResponseCommand = ReactiveCommand.Create(() => StartMakeQuestionnaireResponse(), canExecuteMakeQuestionnaireResponse);
        }
        #endregion
        #region Methods
        public async void StartCommandGetQuestions()
        {
            QuestionnaireResult = string.Empty;
            QuestionnaireResult = await System.Threading.Tasks.Task.Factory.StartNew(() => GetqQuestionnaire());
            try
            {                
                GetOrder.AddResource(JsonConvert.DeserializeObject<QuestionnaireLocal>(QuestionnaireResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore })); //, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }
                List<QuestionnaireItem> items = GetOrder.Get().Questionnaire.Item.Items;
                List<QuestionnaireAnswer> sItems = new List<QuestionnaireAnswer>();
                List<QuestionnaireAnswer> cItems = new List<QuestionnaireAnswer>();
                foreach (var item in items)
                {
                    List<QuestionnaireItem> nextItems = item.Items;
                    logger.Info($"Items count: {nextItems.Count}");
                    foreach (var itemAnswer in nextItems)
                    {
                        if (itemAnswer.Type == "choice")
                        {
                            cItems.Add(new QuestionnaireAnswer(itemAnswer, item.LinkId, item.Text));
                        }
                        else
                        {
                            sItems.Add(new QuestionnaireAnswer(itemAnswer, item.LinkId, item.Text));
                        }
                    }
                }
                ChoiceItems = cItems;
                StringItems = sItems;
                logger.Info($"ChoiceItems count: {ChoiceItems.Count}"); 
                logger.Info($"StringItems count: {StringItems.Count}"); 
            }
            catch (Exception ex)
            {
                QuestionnaireResult += $"\r\n\r\nНе удалось добавить ответы на вопросы в заказ по причине:\r\n{ex}";
            }

        }
        private string GetqQuestionnaire()
        {
            string result;
            IsWaiting = true;
            try
            {
                result = GetFromService.Questionnaire(BodyForPreanalytics);
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

        public async void StartMakeQuestionnaireResponse()
        {
            if (ChoiceItems.Where(y => y.ValidateColor == "LightGreen" || y.ValidateColor == "Yellow").Count() == ChoiceItems.Count && StringItems.Where(y => y.ValidateColor == "LightGreen" || y.ValidateColor == "Yellow").Count() == StringItems.Count)
            {
                QuestionnaireResult = await System.Threading.Tasks.Task.Factory.StartNew(() => MakeQuestionnaireResponse());
            }
            else
            {
                QuestionnaireResult = "Не все обязательные поля заполнены корректно!!!";
            }
            
        }
        private string MakeQuestionnaireResponse()
        {
            string result;
            try
            {
                List<QuestionnaireAnswer> allAnswers = StringItems is null ? ChoiceItems is null ? null : ChoiceItems : StringItems;
                if (allAnswers != null && StringItems != null && ChoiceItems != null)
                {
                    allAnswers.AddRange(ChoiceItems);
                }
                result = Make.Answers(allAnswers);
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
        #endregion

    }
}
