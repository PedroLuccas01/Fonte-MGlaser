/*
 * By: Pedro Borges - Delta Sollutions/Conecthus
 * 03/09/2024
 */

//Classe responsavel por realizar a conversão de Excel em TXT para poder utilizar os arquivos no formato XLS com 4 colunas a partir da segunda linha de cada coluna
//por conta do cmp1, cmp2, cmp3 e cmp4

using Prj_RBS;
using System;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

public class ExcelToTxtConverter
{
    /// <summary>
    /// Converte o conteúdo de um arquivo Excel (.XLS) para um arquivo de texto (.txt).
    /// </summary>
    /// <param name="excelFilePath">Caminho do arquivo Excel (.XLS)</param>
    /// <param name="txtFilePath">Caminho do arquivo de saída (.txt)</param>
    public void ConvertXlsToTxt(string excelFilePath, string txtFilePath)
    {
        IniFile file = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");

        Excel.Application excelApp = null;
        Excel.Workbook workbook = null;
        Excel.Worksheet worksheet = null;
        Excel.Range range = null;

        try
        {
            // Inicializa a aplicação Excel
            excelApp = new Excel.Application();
            excelApp.DisplayAlerts = false; // Desativa alertas do Excel

            // Abre o arquivo Excel
            workbook = excelApp.Workbooks.Open(
                excelFilePath,
                ReadOnly: true, // Abre o arquivo como somente leitura
                Editable: false);

            // Obtém a primeira planilha do arquivo Excel
            worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // Obtém o intervalo utilizado na planilha
            range = worksheet.UsedRange;

            // Cria um StreamWriter para escrever no arquivo .txt
            string tempTxtFilePath = Path.Combine(Path.GetDirectoryName(txtFilePath), "Temp_" + Path.GetFileName(txtFilePath));
            using (StreamWriter writer = new StreamWriter(tempTxtFilePath))
            {
                // Loop pelas linhas da planilha, ignorando a primeira linha (cabeçalho)
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    for (int col = 1; col <= range.Columns.Count; col++)
                    {
                        // Obtém o valor da célula e converte para string
                        var cellValue = (range.Cells[row, col] as Excel.Range).Text;

                        //remove espaços em branco
                        var cleanedValue = cellValue.Trim();

                        // Escreve o valor da célula no arquivo de texto
                        if (!string.IsNullOrEmpty(cleanedValue))
                        {
                            writer.WriteLine(cleanedValue);
                        }
                    }
                }
            }

            // Agora move o conteúdo para o arquivo final
            if (File.Exists(txtFilePath))
            {
                File.Delete(txtFilePath); // Remove o arquivo final se já existir
            }

            File.Move(tempTxtFilePath, txtFilePath);

            //// Mensagem confirmando a localização do arquivo final
            Console.WriteLine($"Arquivo: {txtFilePath} - Pronto para uso."/*, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information*/);

            file.Write("cam", txtFilePath, "DIR");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao converter o arquivo Excel: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Libera os recursos do Excel corretamente
            if (range != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
            if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            if (workbook != null)
            {
                workbook.Close(false);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            }
            if (excelApp != null)
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }
    }
}
