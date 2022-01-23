using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CatalogoDeTrabajosDeGraduacion.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;

namespace CatalogoDeTrabajosDeGraduacion.Controllers
{
    public class TrabajosController : Controller
    {
        private readonly TrabajosGraduacionDbContext _context;

        // constructor
        public TrabajosController(TrabajosGraduacionDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Estadisticas(string filtro)
        {
            ViewData["FiltroActual"] = filtro;

            switch (filtro)
            {
                case "Año":
                    var cantidadTrabajosPorYear = (from t in _context.Trabajos select new { t.TrabaFecha.Year })
                        .AsEnumerable()
                        .GroupBy(x => x.Year)
                        .ToDictionary(y => y.Key.ToString(), y => y.Count());

                    ViewBag.Estadisticas = cantidadTrabajosPorYear;

                    return View();

                case "Tipo":
                    var cantidadTrabajosPorTipo = (from tt in _context.TiposTrabajos
                                                   join t in _context.Trabajos on tt.TpTrabaId equals t.TpTrabaId
                                                   select new { tt })
                                                   .AsEnumerable()
                                                   .GroupBy(x => x.tt.TpTrabaNombre)
                                                   .ToDictionary(y => y.Key, y => y.Count());

                    ViewBag.Estadisticas = cantidadTrabajosPorTipo;

                    return View();

                case "Facultad":
                    var cantidadTrabajosPorFacultad = (from t in _context.Trabajos
                                                       join a in _context.Autores on t.TrabaId equals a.TrabaId
                                                       join c in _context.Carreras on a.CarreId equals c.CarreId
                                                       join f in _context.Facultades on c.FaculId equals f.FaculId
                                                       select new { f })
                                                       .AsEnumerable()
                                                       .GroupBy(x => x.f.FaculNombre)
                                                       .ToDictionary(y => y.Key, y => y.Count());

                    ViewBag.Estadisticas = cantidadTrabajosPorFacultad;

                    return View();

                case "Carrera":
                    var cantidadTrabajosPorCarrera = (from t in _context.Trabajos
                                                      join a in _context.Autores on t.TrabaId equals a.TrabaId
                                                      join c in _context.Carreras on a.CarreId equals c.CarreId
                                                      select new { c })
                                                      .AsEnumerable()
                                                      .GroupBy(x => x.c.CarreNombre)
                                                      .ToDictionary(y => y.Key, y => y.Count());

                    ViewBag.Estadisticas = cantidadTrabajosPorCarrera;

                    return View();

                default:
                    return View();
            };
        }

        // GET: Trabajos
        public async Task<IActionResult> Index(string busqueda)
        {
            ViewData["BusquedaActual"] = busqueda;

            var trabajos = _context.Trabajos.Include(t => t.TpTraba).Include(t => t.Autores).ThenInclude(a => a.Carre).ThenInclude(c => c.Facul);
            if (!string.IsNullOrEmpty(busqueda))
            {
                trabajos = trabajos.Where(
                    t => t.TrabaTitulo.Contains(busqueda) ||
                    t.Autores.First().AutorNombre.Contains(busqueda) ||
                    t.Autores.First().AutorApellido.Contains(busqueda) ||
                    t.Autores.First().Carre.Facul.FaculNombre.Contains(busqueda)
                    ).Include(t => t.TpTraba).Include(t => t.Autores).ThenInclude(a => a.Carre).ThenInclude(c => c.Facul);
            }
            return View(await trabajos.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Descargar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NoContent();
            }

            var ruta = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", id + ".pdf");
            var memory = new MemoryStream();
            using var stream = new FileStream(ruta, FileMode.Open);
            await stream.CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(ruta));
        }


        [HttpGet]
        public IActionResult Subir()
        {
            TrabajosModeloPersonalizado trabajo = new TrabajosModeloPersonalizado();
            ViewData["TpTrabaId"] = new SelectList(_context.TiposTrabajos, "TpTrabaId", "TpTrabaNombre");
            ViewData["CarreId"] = new SelectList(_context.Carreras, "CarreId", "CarreNombre");
            return View(trabajo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(TrabajosModeloPersonalizado trabajosModelo, IFormFile file)
        {
            try
            {
                if (file != null && file.Length != 0)
                {
                    if (ModelState.IsValid)
                    {
                        var trabajo = trabajosModelo.Trabajo;
                        var autor = trabajosModelo.Autor;
                        var fileName = $"{DateTime.Today.Year}{DateTime.Today.Month}{DateTime.Today.Day}-T{trabajo.TrabaTitulo}";


                        trabajo.TrabaFecha = DateTime.Today;
                        trabajo.TrabaFile = fileName;
                        _context.Add(trabajo);
                        await _context.SaveChangesAsync();

                        autor.TrabaId = trabajo.TrabaId;
                        _context.Add(autor);
                        await _context.SaveChangesAsync();

                        var ruta = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", fileName + ".pdf");

                        using var stream = new FileStream(ruta, FileMode.Create);
                        await file.CopyToAsync(stream);

                        TempData["alerta"] = "hecho";

                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewData["ErrorMessage"] = "FileError";
                ViewData["TpTrabaId"] = new SelectList(_context.TiposTrabajos, "TpTrabaId", "TpTrabaNombre");
                ViewData["CarreId"] = new SelectList(_context.Carreras, "CarreId", "CarreNombre");
                ViewBag.Message = "El archivo seleccionado no es valido.";
                return View("Subir");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "ExceptionError";
                ViewData["TpTrabaId"] = new SelectList(_context.TiposTrabajos, "TpTrabaId", "TpTrabaNombre");
                ViewData["CarreId"] = new SelectList(_context.Carreras, "CarreId", "CarreNombre");
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Subir");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Administrar(string busqueda)
        {
            ViewData["BusquedaActual"] = busqueda;

            var trabajos = _context.Trabajos.Include(t => t.TpTraba).Include(t => t.Autores).ThenInclude(a => a.Carre).ThenInclude(c => c.Facul);
            if (!string.IsNullOrEmpty(busqueda))
            {
                trabajos = trabajos.Where(
                    t => t.TrabaTitulo.Contains(busqueda) ||
                    t.Autores.First().AutorNombre.Contains(busqueda) ||
                    t.Autores.First().AutorApellido.Contains(busqueda) ||
                    t.Autores.First().Carre.Facul.FaculNombre.Contains(busqueda)
                    ).Include(t => t.TpTraba).Include(t => t.Autores).ThenInclude(a => a.Carre).ThenInclude(c => c.Facul);
            }
            return View(await trabajos.ToListAsync());
        }

        // GET: Trabajos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajo = await _context.Trabajos.Include(t => t.Autores).ThenInclude(a => a.Carre).FirstOrDefaultAsync(t => t.TrabaId == id);

            if (trabajo == null)
            {
                return NotFound();
            }

            var autor = trabajo.Autores.FirstOrDefault();
            var trabajoAutor = new TrabajosModeloPersonalizado
            {
                Trabajo = trabajo,
                Autor = autor
            };

            ViewData["TpTrabaId"] = new SelectList(_context.TiposTrabajos, "TpTrabaId", "TpTrabaNombre", trabajo.TpTrabaId);
            ViewData["CarreId"] = new SelectList(_context.Carreras, "CarreId", "CarreNombre", autor.CarreId);
            return View(trabajoAutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TrabajosModeloPersonalizado trabajosModelo)
        {
            if (ModelState.IsValid)
            {
                var trabajo = trabajosModelo.Trabajo;
                var autor = trabajosModelo.Autor;

                try
                {
                    _context.Update(trabajo);
                    await _context.SaveChangesAsync();

                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrabajoExists(trabajo.TrabaId))
                    {
                        return NotFound();
                    }
                    else if (AutorExists(autor.AutorId))
                    {
                        return NotFound();
                    }
                }

                return RedirectToAction(nameof(Administrar));
            }

            ViewData["TpTrabaId"] = new SelectList(_context.TiposTrabajos, "TpTrabaId", "TpTrabaNombre", trabajosModelo.Trabajo.TpTrabaId);
            ViewData["CarreId"] = new SelectList(_context.Carreras, "CarreId", "CarreNombre", trabajosModelo.Autor.CarreId);

            return View(trabajosModelo);
        }

        // GET: Trabajos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajo = await _context.Trabajos
                .Include(t => t.TpTraba)
                .Include(t => t.Autores)
                .FirstOrDefaultAsync(m => m.TrabaId == id);

            if (trabajo == null)
            {
                return NotFound();
            }

            var autor = trabajo.Autores.FirstOrDefault();

            var trabajoAutor = new TrabajosModeloPersonalizado
            {
                Trabajo = trabajo,
                Autor = autor
            };

            return View(trabajoAutor);
        }

        // POST: Trabajos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(TrabajosModeloPersonalizado trabajosModelo)
        {
            var preTrabajo = trabajosModelo.Trabajo;
            var autor = trabajosModelo.Autor;

            var trabajo = await _context.Trabajos.FindAsync(preTrabajo.TrabaId);
            var preAutor = await _context.Autores.FindAsync(autor.AutorId);

            _context.Autores.Remove(preAutor);
            await _context.SaveChangesAsync();

            _context.Trabajos.Remove(trabajo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Administrar));
        }

        private bool TrabajoExists(int id)
        {
            return _context.Trabajos.Any(e => e.TrabaId == id);
        }
        private bool AutorExists(int id)
        {
            return _context.Autores.Any(a => a.AutorId == id);
        }
    }
}
