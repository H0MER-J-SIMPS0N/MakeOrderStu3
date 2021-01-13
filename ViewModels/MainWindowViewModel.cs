using MakeOrderStu3.Singletones;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MakeOrderStu3.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields and Properties
        public ObservableCollection<ViewModelBase> ViewModels {get;set;}

        private ViewModelBase selectedViewModel;
        public ViewModelBase SelectedViewModel
        {
            get => selectedViewModel;
            set
            {
                if (selectedViewModel != null)
                {
                    selectedViewModel.IsEnabled = false;
                }
                this.RaiseAndSetIfChanged(ref selectedViewModel, value);
                selectedViewModel.IsEnabled = true;                
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            GetOrder.Get();
            ViewModels = new ObservableCollection<ViewModelBase>(new List<ViewModelBase>()
            {
                new NomenclatureViewModel(),
                new SpecimenChoiceViewModel(),
                new PreanalyticsViewModel(),
                new QuestionnaireViewModel(),
                new PatientViewModel(),
                new AssembleWholeOrderViewModel()
            }) ;
            SelectedViewModel = ViewModels is null || ViewModels.Count == 0 ? null : ViewModels[0];
        }
        #endregion
    }
}
