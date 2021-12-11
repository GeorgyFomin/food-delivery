using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
//using WebASP_MVC.Data;
using WebASP_MVC.Helper;
using WebASP_MVC.Models;

namespace WebASP_MVC.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly RestClient restClient = new("https://localhost:7234/");//http://localhost:5234/
        readonly ClientAPI api = new();

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            List<Entities.Delivery>? deliveries = new();
            IRestResponse response = restClient.Get(new RestRequest("api/Deliveries"));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                deliveries = JsonConvert.DeserializeObject<List<Entities.Delivery>>(response.Content);
            }
            //HttpClient client = api.Init();
            //HttpResponseMessage response = await client.GetAsync("api/Deliveries");
            //if (response.IsSuccessStatusCode)
            //{
            //    var result = response.Content.ReadAsStringAsync().Result;
            //    deliveries = JsonConvert.DeserializeObject<List<Entities.Delivery>>(result);
            //}

            return View(deliveries); //await _context.Delivery.ToListAsync());
        }
        IActionResult GetResultById(int? id)
        {
            Entities.Delivery? GetDelivery()
            {
                IRestResponse response = restClient.Get(new RestRequest($"api/Deliveries/{id}"));
                return response.StatusCode != System.Net.HttpStatusCode.OK ? null : JsonConvert.DeserializeObject<Entities.Delivery>(response.Content);
            }
            Entities.Delivery? delivery;
            return id == null || (delivery = GetDelivery()) == null ? NotFound() : View(delivery);
        }
        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id) => GetResultById(id);
        // GET: Deliveries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceName,Price,TimeSpan,Id")] Entities.Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                //IRestRequest request = new RestRequest("api/Deliveries", Method.POST);
                //request.AddJsonBody(delivery);
                //IRestResponse response = restClient.Execute(request);
                HttpClient client = api.Init();
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Deliveries", delivery);
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id) => GetResultById(id);

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceName,Price,TimeSpan,Id")] Entities.Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //IRestRequest request = new RestRequest($"api/Deliveries/{id}", Method.PUT);
                    //request.AddJsonBody(delivery);
                    //IRestResponse response = restClient.Execute(request);
                    HttpClient client = api.Init();
                    HttpResponseMessage response = await client.PutAsJsonAsync($"api/Deliveries/{id}", delivery);
                    response.EnsureSuccessStatusCode();


                    //_context.Update(delivery);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id) => GetResultById(id);
        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var delivery = await _context.Delivery.FindAsync(id);
            //_context.Delivery.Remove(delivery);
            //await _context.SaveChangesAsync();
            IRestResponse response = restClient.Delete(new RestRequest($"api/Deliveries/{id}"));
            // Или
            //HttpClient client = api.Init();
            //HttpResponseMessage response = await client.DeleteAsync($"api/Deliveries/{id}");
            //response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
            //return _context.Delivery.Any(e => e.Id == id);
            return false;
        }
    }
}
