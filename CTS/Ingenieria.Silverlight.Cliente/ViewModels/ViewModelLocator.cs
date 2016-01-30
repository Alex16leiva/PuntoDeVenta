namespace Ingenieria.Silverlight.Cliente.ViewModels
{
    public class ViewModelLocator
    {
        private PlantaViewModel _plantaViewModel;
        public PlantaViewModel PlantaViewModel
        {
            get
            {
                return _plantaViewModel ??
                    (_plantaViewModel = new PlantaViewModel());
            }
        }
    }
}
