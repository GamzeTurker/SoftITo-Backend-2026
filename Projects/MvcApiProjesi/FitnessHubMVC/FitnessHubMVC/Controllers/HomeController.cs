using FitnessHubMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FitnessHubMVC.Controllers
{
    public class HomeController : Controller
    {
      


        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            var members = JsonConvert.DeserializeObject<List<Member>>(
                client.GetAsync("https://localhost:7201/api/Member/GetMembers")
                .Result.Content.ReadAsStringAsync().Result);

            var trainers = JsonConvert.DeserializeObject<List<Trainer>>(
                client.GetAsync("https://localhost:7201/api/Trainer/GetTrainers")
                .Result.Content.ReadAsStringAsync().Result);

            var equipments = JsonConvert.DeserializeObject<List<Equipment>>(
                client.GetAsync("https://localhost:7201/api/Equipment/GetEquipments")
                .Result.Content.ReadAsStringAsync().Result);

          

            // Sayılar
            ViewBag.MemberCount = members.Count;
            ViewBag.TrainerCount = trainers.Count;
            ViewBag.EquipmentCount = equipments.Count;


            // Son 2 kayıt
            ViewBag.LastMembers = members
            .OrderByDescending(x => x.MemberId)
            .Take(2)
            .ToList();
            
            ViewBag.LastTrainers = trainers
                .OrderByDescending(x => x.TrainerId)
                .Take(2)
                .ToList();

            ViewBag.LastEquipments = equipments
                .OrderByDescending(x => x.EquipmentId)
                .Take(2)
                .ToList();
            return View();
        }


    }
}
