using Microsoft.Maui.Controls;  
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APIRESTFULL.Views
{
    public partial class PageCreatePerson : ContentPage
    {
  
        private string _fotoBase64;
        private int? _selectedId;

        public PageCreatePerson()
        {
            InitializeComponent();
        }

        
        private async void btnfoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) { await DisplayAlert("Aviso", "No se capturó la foto", "OK"); return; }

                using var stream = await photo.OpenReadAsync();
                foto.Source = ImageSource.FromStream(() => stream);

                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                _fotoBase64 = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo capturar la foto: {ex.Message}", "OK");
            }
        }

        private async void btnagregar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo == null) { await DisplayAlert("Aviso", "No se seleccionó ninguna foto", "OK"); return; }

                using var stream = await photo.OpenReadAsync();
                foto.Source = ImageSource.FromStream(() => stream);

                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                _fotoBase64 = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo agregar la foto: {ex.Message}", "OK");
            }
        }

        // CREATE 
        private async void btnproceso_Clicked(object sender, EventArgs e)
        {
            var persona = new Models.Personas
            {
                nombres = Nombres.Text,
                apellidos = Apellidos.Text,
                direccion = Direccion.Text,
                telefono = Telefono.Text,
                edad = int.TryParse(Edad.Text, out var edad) ? edad : 0,
                foto = _fotoBase64 ?? "" // envía base64 o vacío
            };

            var resultado = await Controllers.PersonasController.CreatePerson(persona);
            if (resultado?.success == true)
            {
                await DisplayAlert("Éxito", resultado.message ?? "Registro creado", "OK");
                LimpiarCampos();
            }
            else
            {
                await DisplayAlert("Error", resultado?.message ?? "No se pudo crear", "OK");
            }
        }

        // READ 
        private async void btnleer_Clicked(object sender, EventArgs e)
        {
            var personas = await Controllers.PersonasController.ReadPersons();
            if (personas == null || personas.Count == 0)
            {
                await DisplayAlert("Aviso", "No hay registros", "OK");
                return;
            }

            var opciones = personas.Select(p => $"{p.Id} - {p.nombres} {p.apellidos}").ToArray();
            var elegido = await DisplayActionSheet("Selecciona persona", "Cancelar", null, opciones);
            if (string.IsNullOrWhiteSpace(elegido) || elegido == "Cancelar") return;

            var idStr = elegido.Split(" - ")[0];
            if (!int.TryParse(idStr, out var id)) return;

            var personaSel = personas.FirstOrDefault(p => p.Id == id);
            if (personaSel == null) return;

           
            _selectedId = personaSel.Id;
            Nombres.Text = personaSel.nombres;
            Apellidos.Text = personaSel.apellidos;
            Direccion.Text = personaSel.direccion;
            Telefono.Text = personaSel.telefono;
            Edad.Text = personaSel.edad.ToString();

            if (!string.IsNullOrEmpty(personaSel.foto))
            {
                try
                {
                    var bytes = Convert.FromBase64String(personaSel.foto);
                    foto.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
                    _fotoBase64 = personaSel.foto;
                }
                catch { }
            }
        }

        // UPDATE 
        private async void btnupdate_Clicked(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                await DisplayAlert("Error", "Primero lee y selecciona una persona", "OK");
                return;
            }

            var persona = new Models.Personas
            {
                Id = _selectedId.Value,
                nombres = Nombres.Text,
                apellidos = Apellidos.Text,
                direccion = Direccion.Text,
                telefono = Telefono.Text,
                edad = int.TryParse(Edad.Text, out var edad) ? edad : 0,
                foto = _fotoBase64 ?? ""
            };

            var resultado = await Controllers.PersonasController.UpdatePerson(persona);
            await DisplayAlert(resultado?.success == true ? "Éxito" : "Error", resultado?.message ?? "No se pudo actualizar", "OK");
        }

        // DELETE 
        private async void btndelete_Clicked(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                await DisplayAlert("Error", "Primero lee y selecciona una persona", "OK");
                return;
            }

            var confirmar = await DisplayAlert("Confirmar", "¿Eliminar esta persona?", "Sí", "No");
            if (!confirmar) return;

            var resultado = await Controllers.PersonasController.DeletePerson(_selectedId.Value);
            if (resultado?.success == true)
            {
                await DisplayAlert("Éxito", resultado.message ?? "Eliminado", "OK");
                LimpiarCampos();
                _selectedId = null;
            }
            else
            {
                await DisplayAlert("Error", resultado?.message ?? "No se pudo eliminar", "OK");
            }
        }

        private void LimpiarCampos()
        {
            Nombres.Text = Apellidos.Text = Direccion.Text = Telefono.Text = string.Empty;
            Edad.Text = string.Empty;
            foto.Source = null;
            _fotoBase64 = null;
        }
    }
}