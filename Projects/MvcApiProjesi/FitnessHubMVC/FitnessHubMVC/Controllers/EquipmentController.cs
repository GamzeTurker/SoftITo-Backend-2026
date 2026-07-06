using FitnessHubMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitnessHubMVC.Controllers
{
    public class EquipmentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync("https://localhost:7201/api/Equipment/GetEquipments").Result;
            List<Equipment> equipments = JsonConvert.DeserializeObject<List<Equipment>>(response.Content.ReadAsStringAsync().Result);
            return View(equipments);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Equipment());
        }

        [HttpPost]
        public IActionResult Create(Equipment equipment)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(equipment), System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync("https://localhost:7201/api/Equipment/AddEquipments", content).Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla eklendi!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Equipment/GetEquipmentsById/{id}").Result;
            var equipments = JsonConvert.DeserializeObject<Equipment>(response.Content.ReadAsStringAsync().Result);
            return View(equipments);
        }

        [HttpPost]
        public IActionResult Edit(Equipment equipment)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(equipment), System.Text.Encoding.UTF8, "application/json");
            var response = client.PutAsync($"https://localhost:7201/api/Equipment/UpdateEquipments/{equipment.EquipmentId}", content).Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Equipment/GetEquipmentsById/{id}").Result;
            var equipment = JsonConvert.DeserializeObject<Equipment>(response.Content.ReadAsStringAsync().Result);
            return View(equipment);
        }
        [HttpPost]
        public IActionResult Delete(Equipment equipment)
        {
            HttpClient client = new HttpClient();
            var response = client.DeleteAsync($"https://localhost:7201/api/Equipment/DeleteEquipments/{equipment.EquipmentId}").Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
    }
}
