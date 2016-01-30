using Ingenieria.Silverlight.Cliente.PlantaService;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ingenieria.Silverlight.Cliente.ViewModels
{
    public class PlantaViewModel : BaseINPC
    {
        #region Miembros Privados
        private readonly PlantaModelServiceClient  _plantaProxy;
        #endregion

        #region contructor
        public PlantaViewModel()
        {
            _plantaProxy = new PlantaModelServiceClient();
            ObtenerPlantas();
        }
        #endregion

        #region Metodos
        private void ObtenerPlantas()
        {
            _plantaProxy.ObtenerPlantasAsync();
            _plantaProxy.ObtenerPlantasCompleted += ObtenerPlantasResult;
        }

        private void ObtenerPlantasResult(object sender, ObtenerPlantasCompletedEventArgs e)
        {
            ListaPlantas = e.Result;
        }
        #endregion

        #region Propiedades
        public ObservableCollection<PlantaDTO> ListaPlantas
        {
            get { return _listaPlantas; }
            set
            {
                if (_listaPlantas != value)
                {
                    _listaPlantas = value;
                    RaisePropertyChanged("ListaPlantas");
                }
            }
        }
        private ObservableCollection<PlantaDTO> _listaPlantas;

        #endregion

    }
}
