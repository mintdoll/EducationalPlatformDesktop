using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class CertificateViewModel : ViewModelBase
    {
        private Certificate? _selectedCertificate;

        public CertificateViewModel(
            ObservableCollection<Certificate> certificates)
        {
            Certificates = certificates;

            SelectCertificateCommand =
                new RelayCommand(parameter =>
                    SelectedCertificate = parameter as Certificate);

            Certificates.CollectionChanged +=
                OnCertificatesCollectionChanged;

            if (Certificates.Count > 0)
            {
                SelectedCertificate = Certificates[0];
            }
        }

        public ObservableCollection<Certificate> Certificates { get; }

        public Certificate? SelectedCertificate
        {
            get => _selectedCertificate;
            set
            {
                if (_selectedCertificate == value)
                {
                    return;
                }

                _selectedCertificate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedCertificate));
            }
        }

        public bool HasCertificates =>
            Certificates.Count > 0;

        public bool IsEmpty =>
            Certificates.Count == 0;

        public bool HasSelectedCertificate =>
            SelectedCertificate != null;

        public RelayCommand SelectCertificateCommand { get; }

        private void OnCertificatesCollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasCertificates));
            OnPropertyChanged(nameof(IsEmpty));

            if (SelectedCertificate == null &&
                Certificates.Count > 0)
            {
                SelectedCertificate = Certificates[0];
            }
        }
    }
}