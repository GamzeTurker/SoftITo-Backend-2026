using FitnessHubMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitnessHubMVC.Controllers
{
    public class TrainerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync("https://localhost:7201/api/Trainer/GetTrainers").Result;
            List<Trainer> trainers = JsonConvert.DeserializeObject<List<Trainer>>(response.Content.ReadAsStringAsync().Result);
            return View(trainers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Trainer());
        }

        [HttpPost]
        public IActionResult Create(Trainer trainer)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(trainer), System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync("https://localhost:7201/api/Trainer/AddTrainers", content).Result;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Trainer/GetTrainersById/{id}").Result;
            var trainers = JsonConvert.DeserializeObject<Trainer>(response.Content.ReadAsStringAsync().Result);
            return View(trainers);
        }

        [HttpPost]
        public IActionResult Edit(Trainer trainer)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(trainer), System.Text.Encoding.UTF8, "application/json");
            var response = client.PutAsync($"https://localhost:7201/api/Trainer/UpdateTrainers/{trainer.TrainerId}", content).Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Trainer/GetTrainersById/{id}").Result;
            var trainer = JsonConvert.DeserializeObject<Trainer>(response.Content.ReadAsStringAsync().Result);
            return View(trainer);
        }
        [HttpPost]
        public IActionResult Delete(Trainer trainer)
        {
            HttpClient client = new HttpClient();
            var response = client.DeleteAsync($"https://localhost:7201/api/Trainer/DeleteTrainers/{trainer.TrainerId}").Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}
