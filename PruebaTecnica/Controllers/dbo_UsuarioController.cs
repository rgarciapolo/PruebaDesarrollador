using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using PagedList;
using PagedList.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PruebaTecnica.Models;

namespace PruebaTecnica.Controllers
{
    public class dbo_UsuarioController : Controller
    {
        private DBSystemDBContext db = new DBSystemDBContext();
        
        // GET: /dbo_Usuario/
        public ActionResult Index(string sortOrder,  
                                  String SearchField,
                                  String SearchCondition,
                                  String SearchText,
                                  String Export,
                                  int? PageSize,
                                  int? page, 
                                  string command)
        {

            if (command == "Mostrar Todo") {
                SearchField = null;
                SearchCondition = null;
                SearchText = null;
                Session["SearchField"] = null;
                Session["SearchCondition"] = null;
                Session["SearchText"] = null; } 
            else if (command == "Insertar") { return RedirectToAction("Insertar"); } 
            else if (command == "Exportar") { Session["Exportar"] = Export; } 
            else if (command == "Buscar" | command == "Cantidad de Registros") {
                if (!string.IsNullOrEmpty(SearchText)) {
                    Session["SearchField"] = SearchField;
                    Session["SearchCondition"] = SearchCondition;
                    Session["SearchText"] = SearchText; }
                } 
            if (command == "Cantidad de Registros") { Session["PageSize"] = PageSize; }

            ViewData["SearchFields"] = GetFields((Session["SearchField"] == null ? "Usuario" : Convert.ToString(Session["SearchField"])));
            ViewData["SearchConditions"] = Library.GetConditions((Session["SearchCondition"] == null ? "Contiene" : Convert.ToString(Session["SearchCondition"])));
            ViewData["SearchText"] = Session["SearchText"];
            ViewData["Exports"] = Library.GetExports((Session["Exportar"] == null ? "Pdf" : Convert.ToString(Session["Exportar"])));
            ViewData["PageSizes"] = Library.GetPageSizes();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdUsuarioSortParm"] = sortOrder == "IdUsuario_asc" ? "IdUsuario_desc" : "IdUsuario_asc";
            ViewData["NombreSortParm"] = sortOrder == "Nombre_asc" ? "Nombre_desc" : "Nombre_asc";
            ViewData["DireccionSortParm"] = sortOrder == "Direccion_asc" ? "Direccion_desc" : "Direccion_asc";
            ViewData["FechaNacimientoSortParm"] = sortOrder == "FechaNacimiento_asc" ? "FechaNacimiento_desc" : "FechaNacimiento_asc";
            ViewData["IdCiudadNacimientoSortParm"] = sortOrder == "IdCiudadNacimiento_asc" ? "IdCiudadNacimiento_desc" : "IdCiudadNacimiento_asc";
            ViewData["IdTipoDocumentoSortParm"] = sortOrder == "IdTipoDocumento_asc" ? "IdTipoDocumento_desc" : "IdTipoDocumento_asc";
            ViewData["NumeroDocumentoSortParm"] = sortOrder == "NumeroDocumento_asc" ? "NumeroDocumento_desc" : "NumeroDocumento_asc";
            ViewData["IdCiudadSortParm"] = sortOrder == "IdCiudad_asc" ? "IdCiudad_desc" : "IdCiudad_asc";

            var Query = db.dbo_Usuario.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contiene") {
                        Query = Query.Where(p => 
                                                 ("Usuario".ToString().Equals(SearchField) && p.IdUsuario.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Nombre".ToString().Equals(SearchField) && p.Nombre.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Direccion".ToString().Equals(SearchField) && p.Direccion.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Fecha Nacimiento".ToString().Equals(SearchField) && p.FechaNacimiento.Value.ToString().Contains(SearchText)) 
                                                 || ("Ciudad Nacimiento".ToString().Equals(SearchField) && p.dbo_Ciudad.Descripcion.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Tipo Documento".ToString().Equals(SearchField) && p.dbo_TipoDocumento.Descripcion.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Numero Documento".ToString().Equals(SearchField) && p.NumeroDocumento.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Ciudad Documento".ToString().Equals(SearchField) && p.IdCiudad.Value.ToString().Contains(SearchText)) 
                                         );
                    } else if (SearchCondition == "Inicia en...") {
                        Query = Query.Where(p => 
                                                 ("Usuario".ToString().Equals(SearchField) && p.IdUsuario.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Nombre".ToString().Equals(SearchField) && p.Nombre.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Direccion".ToString().Equals(SearchField) && p.Direccion.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Fecha Nacimiento".ToString().Equals(SearchField) && p.FechaNacimiento.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Ciudad Nacimiento".ToString().Equals(SearchField) && p.dbo_Ciudad.Descripcion.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Tipo Documento".ToString().Equals(SearchField) && p.dbo_TipoDocumento.Descripcion.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Numero Documento".ToString().Equals(SearchField) && p.NumeroDocumento.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Ciudad Documento".ToString().Equals(SearchField) && p.IdCiudad.Value.ToString().StartsWith(SearchText)) 
                                         );
                    } else if (SearchCondition == "Igual a") {
                        if ("Usuario".Equals(SearchField)) { var mIdUsuario = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdUsuario == mIdUsuario); }
                        else if ("Nombre".Equals(SearchField)) { var mNombre = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Nombre == mNombre); }
                        else if ("Direccion".Equals(SearchField)) { var mDireccion = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Direccion == mDireccion); }
                        else if ("Fecha Nacimiento".Equals(SearchField)) { var mFechaNacimiento = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.FechaNacimiento == mFechaNacimiento); }
                        else if ("Ciudad Nacimiento".Equals(SearchField)) { var mIdCiudadNacimiento = System.Convert.ToString(SearchText); Query = Query.Where(p => p.dbo_Ciudad.Descripcion == mIdCiudadNacimiento); }
                        else if ("Tipo Documento".Equals(SearchField)) { var mIdTipoDocumento = System.Convert.ToString(SearchText); Query = Query.Where(p => p.dbo_TipoDocumento.Descripcion == mIdTipoDocumento); }
                        else if ("Numero Documento".Equals(SearchField)) { var mNumeroDocumento = System.Convert.ToString(SearchText); Query = Query.Where(p => p.NumeroDocumento == mNumeroDocumento); }
                        else if ("Ciudad Documento".Equals(SearchField)) { var mIdCiudad = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdCiudad == mIdCiudad); }
                    } else if (SearchCondition == "Mayor que...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Usuario".Equals(SearchField)) { var mIdUsuario = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdUsuario > mIdUsuario); }
                        else if ("Fecha Nacimiento".Equals(SearchField)) { var mFechaNacimiento = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.FechaNacimiento > mFechaNacimiento); }
                        else if ("Ciudad Documento".Equals(SearchField)) { var mIdCiudad = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdCiudad > mIdCiudad); }
                    } else if (SearchCondition == "Menor que...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Usuario".Equals(SearchField)) { var mIdUsuario = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdUsuario < mIdUsuario); }
                        else if ("Fecha Nacimiento".Equals(SearchField)) { var mFechaNacimiento = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.FechaNacimiento < mFechaNacimiento); }
                        else if ("Ciudad Documento".Equals(SearchField)) { var mIdCiudad = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdCiudad < mIdCiudad); }
                    } else if (SearchCondition == "Mayor o igual que...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Usuario".Equals(SearchField)) { var mIdUsuario = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdUsuario >= mIdUsuario); }
                        else if ("Fecha Nacimiento".Equals(SearchField)) { var mFechaNacimiento = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.FechaNacimiento >= mFechaNacimiento); }
                        else if ("Ciudad Documento".Equals(SearchField)) { var mIdCiudad = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdCiudad >= mIdCiudad); }
                    } else if (SearchCondition == "Menor o igual que...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Usuario".Equals(SearchField)) { var mIdUsuario = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdUsuario <= mIdUsuario); }
                        else if ("Fecha Nacimiento".Equals(SearchField)) { var mFechaNacimiento = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.FechaNacimiento <= mFechaNacimiento); }
                        else if ("Ciudad Documento".Equals(SearchField)) { var mIdCiudad = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.IdCiudad <= mIdCiudad); }
                    }
                }
            } catch (Exception) { }

            switch (sortOrder)
            {
                case "IdUsuario_desc":
                    Query = Query.OrderByDescending(s => s.IdUsuario);
                    break;
                case "IdUsuario_asc":
                    Query = Query.OrderBy(s => s.IdUsuario);
                    break;
                case "Nombre_desc":
                    Query = Query.OrderByDescending(s => s.Nombre);
                    break;
                case "Nombre_asc":
                    Query = Query.OrderBy(s => s.Nombre);
                    break;
                case "Direccion_desc":
                    Query = Query.OrderByDescending(s => s.Direccion);
                    break;
                case "Direccion_asc":
                    Query = Query.OrderBy(s => s.Direccion);
                    break;
                case "FechaNacimiento_desc":
                    Query = Query.OrderByDescending(s => s.FechaNacimiento);
                    break;
                case "FechaNacimiento_asc":
                    Query = Query.OrderBy(s => s.FechaNacimiento);
                    break;
                case "IdCiudadNacimiento_desc":
                    Query = Query.OrderByDescending(s => s.dbo_Ciudad.Descripcion);
                    break;
                case "IdCiudadNacimiento_asc":
                    Query = Query.OrderBy(s => s.dbo_Ciudad.Descripcion);
                    break;
                case "IdTipoDocumento_desc":
                    Query = Query.OrderByDescending(s => s.dbo_TipoDocumento.Descripcion);
                    break;
                case "IdTipoDocumento_asc":
                    Query = Query.OrderBy(s => s.dbo_TipoDocumento.Descripcion);
                    break;
                case "NumeroDocumento_desc":
                    Query = Query.OrderByDescending(s => s.NumeroDocumento);
                    break;
                case "NumeroDocumento_asc":
                    Query = Query.OrderBy(s => s.NumeroDocumento);
                    break;
                case "IdCiudad_desc":
                    Query = Query.OrderByDescending(s => s.dbo_Ciudad.Id);
                    break;
                case "IdCiudad_asc":
                    Query = Query.OrderBy(s => s.dbo_Ciudad.Id);
                    break;
                default:  // Name ascending 
                    Query = Query.OrderBy(s => s.IdUsuario);
                    break;
            }

            if (command == "Exportar") {
                GridView gv = new GridView();
                DataTable dt = new DataTable();
                dt.Columns.Add("Usuario", typeof(string));
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Direccion", typeof(string));
                dt.Columns.Add("Fecha Nacimiento", typeof(string));
                dt.Columns.Add("Ciudad Nacimiento", typeof(string));
                dt.Columns.Add("Tipo Documento", typeof(string));
                dt.Columns.Add("Numero Documento", typeof(string));
                dt.Columns.Add("Ciudad Documento", typeof(string));
                foreach (var item in Query.ToList())
                {
                    dt.Rows.Add(
                        item.IdUsuario
                       ,item.Nombre
                       ,item.Direccion
                       ,item.FechaNacimiento
                       ,item.dbo_Ciudad.Descripcion
                       ,item.dbo_TipoDocumento.Descripcion
                       ,item.NumeroDocumento
                       ,item.dbo_Ciudad.Id
                    );
                }
                gv.DataSource = dt;
                gv.DataBind();
                ExportData(Export, gv, dt);
            }

            int pageNumber = (page ?? 1);
            int? pageSZ = (Convert.ToInt32(Session["PageSize"]) == 0 ? 5 : Convert.ToInt32(Session["PageSize"]));
            return View(Query.ToPagedList(pageNumber, (pageSZ ?? 5)));
        }

        // GET: /dbo_Usuario/Details/<id>
        public ActionResult Details(
                                      Int32? IdUsuario
                                   )
        {
            if (
                    IdUsuario == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbo_Usuario dbo_Usuario = db.dbo_Usuario.Find(
                                                 IdUsuario
                                            );
            if (dbo_Usuario == null)
            {
                return HttpNotFound();
            }
            return View(dbo_Usuario);
        }

        // GET: /dbo_Usuario/Create
        public ActionResult Create()
        {
        // ComboBox
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Descripcion");
            ViewData["Id"] = new SelectList(db.dbo_TipoDocumento, "Id", "Descripcion");
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Id");

            return View();
        }

        // POST: /dbo_Usuario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "IdUsuario"
				   + "," + "Nombre"
				   + "," + "Direccion"
				   + "," + "FechaNacimiento"
				   + "," + "IdCiudadNacimiento"
				   + "," + "IdTipoDocumento"
				   + "," + "NumeroDocumento"
				   + "," + "IdCiudad"
				  )] dbo_Usuario dbo_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.dbo_Usuario.Add(dbo_Usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        // ComboBox
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Descripcion", dbo_Usuario.IdCiudadNacimiento);
            ViewData["Id"] = new SelectList(db.dbo_TipoDocumento, "Id", "Descripcion", dbo_Usuario.IdTipoDocumento);
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Id", dbo_Usuario.IdCiudad);

            return View(dbo_Usuario);
        }

        // GET: /dbo_Usuario/Edit/<id>
        public ActionResult Edit(
                                   Int32? IdUsuario
                                )
        {
            if (
                    IdUsuario == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbo_Usuario dbo_Usuario = db.dbo_Usuario.Find(
                                                 IdUsuario
                                            );
            if (dbo_Usuario == null)
            {
                return HttpNotFound();
            }
        // ComboBox
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Descripcion", dbo_Usuario.IdCiudadNacimiento);
            ViewData["Id"] = new SelectList(db.dbo_TipoDocumento, "Id", "Descripcion", dbo_Usuario.IdTipoDocumento);
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Id", dbo_Usuario.IdCiudad);

            return View(dbo_Usuario);
        }

        // POST: /dbo_Usuario/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(dbo_Usuario dbo_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dbo_Usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        // ComboBox
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Descripcion", dbo_Usuario.IdCiudadNacimiento);
            ViewData["Id"] = new SelectList(db.dbo_TipoDocumento, "Id", "Descripcion", dbo_Usuario.IdTipoDocumento);
            ViewData["Id"] = new SelectList(db.dbo_Ciudad, "Id", "Id", dbo_Usuario.IdCiudad);

            return View(dbo_Usuario);
        }

        // GET: /dbo_Usuario/Delete/<id>
        public ActionResult Delete(
                                     Int32? IdUsuario
                                  )
        {
            if (
                    IdUsuario == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbo_Usuario dbo_Usuario = db.dbo_Usuario.Find(
                                                 IdUsuario
                                            );
            if (dbo_Usuario == null)
            {
                return HttpNotFound();
            }
            return View(dbo_Usuario);
        }

        // POST: /dbo_Usuario/Delete/<id>
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Int32? IdUsuario
                                            )
        {
            dbo_Usuario dbo_Usuario = db.dbo_Usuario.Find(
                                                 IdUsuario
                                            );
            db.dbo_Usuario.Remove(dbo_Usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private static List<SelectListItem> GetFields(String select)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem Item1 = new SelectListItem { Text = "Usuario", Value = "Usuario" };
            SelectListItem Item2 = new SelectListItem { Text = "Nombre", Value = "Nombre" };
            SelectListItem Item3 = new SelectListItem { Text = "Direccion", Value = "Direccion" };
            SelectListItem Item4 = new SelectListItem { Text = "Fecha Nacimiento", Value = "Fecha Nacimiento" };
            SelectListItem Item5 = new SelectListItem { Text = "Ciudad Nacimiento", Value = "Ciudad Nacimiento" };
            SelectListItem Item6 = new SelectListItem { Text = "Tipo Documento", Value = "Tipo Documento" };
            SelectListItem Item7 = new SelectListItem { Text = "Numero Documento", Value = "Numero Documento" };
            SelectListItem Item8 = new SelectListItem { Text = "Ciudad Documento", Value = "Ciudad Documento" };

                 if (select == "Usuario") { Item1.Selected = true; }
            else if (select == "Nombre") { Item2.Selected = true; }
            else if (select == "Direccion") { Item3.Selected = true; }
            else if (select == "Fecha Nacimiento") { Item4.Selected = true; }
            else if (select == "Ciudad Nacimiento") { Item5.Selected = true; }
            else if (select == "Tipo Documento") { Item6.Selected = true; }
            else if (select == "Numero Documento") { Item7.Selected = true; }
            else if (select == "Ciudad Documento") { Item8.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);
            list.Add(Item5);
            list.Add(Item6);
            list.Add(Item7);
            list.Add(Item8);

            return list.ToList();
        }

        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Registrar Usuarios", "Many");
                Document document = pdfForm.CreateDocument();
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                MemoryStream stream = new MemoryStream();
                renderer.PdfDocument.Save(stream, false);

                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + "Report.pdf");
                Response.ContentType = "application/Pdf.pdf";
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }
            else
            {
                Response.ClearContent();
                Response.Buffer = true;
                if (Export == "Excel")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "Report.xls");
                    Response.ContentType = "application/Excel.xls";
                }
                else if (Export == "Word")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "Report.doc");
                    Response.ContentType = "application/Word.doc";
                }
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

    }
}
 
