using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

public class PDFform
{
    public PDFform()
    {
    }

    private Document document;
    private DataTable dt;
    private Table table;
    private string Title;

    private string OneORMany;
    public PDFform(DataTable dtIn, String Name, string Type)
    {
        dt = dtIn;
        Title = Name;
        OneORMany = Type;
    }

    public Document CreateDocument()
    {
        this.document = new Document();
        DefineStyles();
        CreatePage(OneORMany);
        FillContent();
        return this.document;
    }

    void DefineStyles()
    {
        Style style = this.document.Styles["Normal"];
        style.Font.Name = "Verdana";

        style = this.document.Styles.AddStyle("Table", "Normal");
        style.Font.Name = "Verdana";
        style.Font.Name = "Times New Roman";
        style.Font.Size = 8;

        style = this.document.Styles.AddStyle("Reference", "Normal");
        style.ParagraphFormat.SpaceAfter = "5mm";
    }

    void CreatePage(string Type)
    {
        PageSetup pageSetup = this.document.DefaultPageSetup.Clone();
        if (OneORMany == "One")
        {
            pageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
        }
        else
        {
            pageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
        }
        Section section = this.document.AddSection();
        section.PageSetup.Orientation = pageSetup.Orientation;
        Paragraph paragraph = section.Headers.Primary.AddParagraph();
        paragraph.Style = "Reference";
        paragraph.AddFormattedText(Title, TextFormat.Bold);

        this.table = section.AddTable();
        this.table.Style = "Table";
        this.table.Borders.Width = 0.25;
        this.table.Borders.Left.Width = 0.5;
        this.table.Borders.Right.Width = 0.5;
        this.table.Rows.LeftIndent = 0;

        Column column = default(Column);
        if (OneORMany == "One")
        {
            column = this.table.AddColumn(100);
            column.Format.Alignment = ParagraphAlignment.Center;
            column = this.table.AddColumn(350);
            column.Format.Alignment = ParagraphAlignment.Center;
        }
        else
        {
            foreach (DataColumn col in dt.Columns)
            {
                column = this.table.AddColumn();
                column.Format.Alignment = ParagraphAlignment.Center;
            }
        }

        // Create the header of the table
        Row row = table.AddRow();
        row.HeadingFormat = true;
        row.Format.Alignment = ParagraphAlignment.Center;
        row.Format.Font.Bold = true;
        row.Shading.Color = TableBlue;

        for (int i = 0; i <= dt.Columns.Count - 1; i++)
        {
            row.Cells[i].AddParagraph(dt.Columns[i].ColumnName);
            row.Cells[i].Format.Font.Bold = true;
            row.Cells[i].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[i].VerticalAlignment = VerticalAlignment.Bottom;
        }

        this.table.SetEdge(0, 0, dt.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75);
    }

    void FillContent()
    {
        Row row1 = default(Row);
        for (int i = 0; i <= dt.Rows.Count - 1; i++)
        {
            row1 = this.table.AddRow();
            row1.TopPadding = 1.5;
            for (int j = 0; j <= dt.Columns.Count - 1; j++)
            {
                row1.Cells[j].Shading.Color = Color.Empty;
                row1.Cells[j].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[j].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[j].Format.FirstLineIndent = 1;
                row1.Cells[j].AddParagraph(dt.Rows[i][j].ToString());
                this.table.SetEdge(0, this.table.Rows.Count - 2, dt.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75);
            }
        }
    }

    // Some pre-defined colors
#if true
    // RGB colors
    readonly static Color TableBorder = new Color(81, 125, 192);
    readonly static Color TableBlue = new Color(235, 240, 249);
    readonly static Color TableGray = new Color(242, 242, 242);
#else
    // CMYK colors
    readonly static Color tableBorder = Color.FromCmyk(100, 50, 0, 30);
    readonly static Color tableBlue = Color.FromCmyk(0, 80, 50, 30);
    readonly static Color tableGray = Color.FromCmyk(30, 0, 0, 0, 100);
#endif
}