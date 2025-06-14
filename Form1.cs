
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class Form1 : Form
    {
        TextBox[,] grid = new TextBox[9, 9];

        public Form1()
        {
            InitializeComponent();
            CriarGrade();
            CriarBotaoResolver();
        }

        private void CriarGrade()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox box = new TextBox();
                    box.Width = 35;
                    box.Height = 35;
                    box.TextAlign = HorizontalAlignment.Center;
                    box.Font = new Font("Arial", 14);
                    box.Location = new Point(j * 38 + 10, i * 38 + 10);
                    grid[i, j] = box;
                    Controls.Add(box);
                }
            }
        }

        private void CriarBotaoResolver()
        {
            Button resolver = new Button();
            resolver.Text = "Resolver Sudoku";
            resolver.Font = new Font("Arial", 12);
            resolver.Location = new Point(10, 350);
            resolver.Click += async (s, e) =>
            {
                int[,] tabuleiro = new int[9, 9];
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        int.TryParse(grid[i, j].Text, out tabuleiro[i, j]);

                await Resolver(tabuleiro);
            };
            Controls.Add(resolver);
        }

        private async Task<bool> Resolver(int[,] tabuleiro)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    grid[i, j].BackColor = Color.White;

            return await Task.Run(async () =>
            {
                return await ResolverBacktracking(tabuleiro, 0, 0);
            });
        }

        private async Task<bool> ResolverBacktracking(int[,] tab, int linha, int coluna)
        {
            if (linha == 9)
                return true;

            int proximaLinha = (coluna == 8) ? linha + 1 : linha;
            int proximaColuna = (coluna + 1) % 9;

            if (tab[linha, coluna] != 0)
                return await ResolverBacktracking(tab, proximaLinha, proximaColuna);

            for (int num = 1; num <= 9; num++)
            {
                if (PodeColocar(tab, linha, coluna, num))
                {
                    tab[linha, coluna] = num;
                    Invoke(new Action(() =>
                    {
                        grid[linha, coluna].Text = num.ToString();
                        grid[linha, coluna].BackColor = Color.LightGreen;
                    }));
                    await Task.Delay(50);
                    if (await ResolverBacktracking(tab, proximaLinha, proximaColuna))
                        return true;
                    tab[linha, coluna] = 0;
                    Invoke(new Action(() =>
                    {
                        grid[linha, coluna].Text = "";
                        grid[linha, coluna].BackColor = Color.LightCoral;
                    }));
                    await Task.Delay(50);
                }
            }
            return false;
        }

        private bool PodeColocar(int[,] tab, int linha, int coluna, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (tab[linha, i] == num || tab[i, coluna] == num)
                    return false;
            }
            int inicioLinha = (linha / 3) * 3;
            int inicioColuna = (coluna / 3) * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (tab[inicioLinha + i, inicioColuna + j] == num)
                        return false;
            return true;
        }
    }
}
