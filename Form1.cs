using System.Diagnostics;

namespace SUDOKUPROJESİ

{ 



    public partial class Form1 : Form
    {
        private readonly TextBox[,] cells = new TextBox[9, 9];
        private readonly int[,] board = new int[9, 9];

        private readonly Random random = new Random();
        private readonly Stopwatch solveStopwatch = new Stopwatch();

        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();

        private Label timeLabel = null!;
        private Label solveTimeLabel = null!;
        private Label statusLabel = null!;
        private ComboBox difficultyComboBox = null!;

        private Button solveButton = null!;
        private Button clearButton = null!;
        private Button newGameButton = null!;

        private int gameSeconds = 0;

        public Form1()
        {
            InitializeComponent();

            CreateInterface();

            CreateBoard();

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            StartNewGame();
        }
        private void CreateInterface()
        {
            this.Text = "Sudoku Solver";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 720);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // -----------------------------
            // BAŞLIK
            // -----------------------------

            Label titleLabel = new Label();

            titleLabel.Text = "SUDOKU";
            titleLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Left = 285;
            titleLabel.Top = 15;

            this.Controls.Add(titleLabel);


            // -----------------------------
            // ZORLUK ETİKETİ
            // -----------------------------

            Label difficultyLabel = new Label();

            difficultyLabel.Text = "Zorluk:";
            difficultyLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            difficultyLabel.Left = 105;
            difficultyLabel.Top = 590;
            difficultyLabel.AutoSize = true;

            this.Controls.Add(difficultyLabel);


            // -----------------------------
            // ZORLUK SEÇİMİ
            // -----------------------------

            difficultyComboBox = new ComboBox();

            difficultyComboBox.Left = 165;
            difficultyComboBox.Top = 585;
            difficultyComboBox.Width = 100;

            difficultyComboBox.DropDownStyle =
                ComboBoxStyle.DropDownList;

            difficultyComboBox.Items.Add("Kolay");
            difficultyComboBox.Items.Add("Orta");
            difficultyComboBox.Items.Add("Zor");

            difficultyComboBox.SelectedIndex = 1;
            

            this.Controls.Add(difficultyComboBox);


            // -----------------------------
            // ÇÖZ BUTONU
            // -----------------------------

            solveButton = new Button();

            solveButton.Text = "ÇÖZ";
            solveButton.Width = 90;
            solveButton.Height = 40;
            solveButton.Left = 285;
            solveButton.Top = 580;

            solveButton.Click += SolveButton_Click;

            this.Controls.Add(solveButton);


            // -----------------------------
            // TEMİZLE BUTONU
            // -----------------------------

            clearButton = new Button();

            clearButton.Text = "TEMİZLE";
            clearButton.Width = 90;
            clearButton.Height = 40;
            clearButton.Left = 385;
            clearButton.Top = 580;

            clearButton.Click += ClearButton_Click;

            this.Controls.Add(clearButton);


            // -----------------------------
            // YENİ SUDOKU
            // -----------------------------

            newGameButton = new Button();

            newGameButton.Text = "YENİ SUDOKU";
            newGameButton.Width = 120;
            newGameButton.Height = 40;
            newGameButton.Left = 485;
            newGameButton.Top = 580;

            newGameButton.Click += NewGameButton_Click;

            this.Controls.Add(newGameButton);


            // -----------------------------
            // OYUN SÜRESİ
            // -----------------------------

            timeLabel = new Label();

            timeLabel.Text = "Oyun Süresi: 00:00";
            timeLabel.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold);

            timeLabel.Left = 105;
            timeLabel.Top = 650;
            timeLabel.AutoSize = true;

            this.Controls.Add(timeLabel);


            // -----------------------------
            // ÇÖZÜM SÜRESİ
            // -----------------------------

            solveTimeLabel = new Label();

            solveTimeLabel.Text = "Çözüm Süresi: -";
            solveTimeLabel.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold);

            solveTimeLabel.Left = 285;
            solveTimeLabel.Top = 650;
            solveTimeLabel.AutoSize = true;

            this.Controls.Add(solveTimeLabel);


            // -----------------------------
            // DURUM
            // -----------------------------

            statusLabel = new Label();

            statusLabel.Text = "Yeni Sudoku hazır!";
            statusLabel.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold);

            statusLabel.Left = 485;
            statusLabel.Top = 650;
            statusLabel.AutoSize = true;

            this.Controls.Add(statusLabel);
        }

        private void CreateBoard()
        {
            int startX = 100;
            int startY = 70;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox cell = new TextBox();

                    cell.Width = 50;
                    cell.Height = 50;

                    int extraX = (col / 3) * 5;
                    int extraY = (row / 3) * 5;

                    cell.Left = startX + col * 55 + extraX;
                    cell.Top = startY + row * 55 + extraY;

                    cell.TextAlign = HorizontalAlignment.Center;

                    cell.Font = new Font(
                        "Arial",
                        20,
                        FontStyle.Bold);

                    cell.MaxLength = 1;

                    cell.BorderStyle = BorderStyle.FixedSingle;

                    cell.KeyPress += Cell_KeyPress;
                    cell.TextChanged += Cell_TextChanged;

                    cells[row, col] = cell;

                    this.Controls.Add(cell);
                }
            }
        }

        // --------------------------------------------------
        // SADECE 1-9 GİRİŞİ
        // --------------------------------------------------

        private void Cell_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar < '1' || e.KeyChar > '9')
            {
                e.Handled = true;
            }
        }

        // --------------------------------------------------
        // HATALI GİRİŞLERİ KONTROL ET
        // --------------------------------------------------

        private void Cell_TextChanged(object? sender, EventArgs e)
        {
            TextBox? changedCell = sender as TextBox;

            if (changedCell == null)
                return;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (cells[row, col] == changedCell)
                    {
                        CheckUserInputs();

                        // Sudoku tamamlandı mı?
                        if (IsSudokuCompleted())
                        {
                            gameTimer.Stop();

                            TimeSpan completionTime =
                                TimeSpan.FromSeconds(gameSeconds);

                            solveTimeLabel.Text =
                                "Çözüm Süresi: " +
                                completionTime.ToString(@"mm\:ss");

                            statusLabel.Text = "Sudoku çözüldü!";

                            MessageBox.Show(
                                "Tebrikler!\n\n" +
                                "Sudoku'yu başarıyla tamamladınız!\n\n" +
                                "Çözüm süresi: " +
                                completionTime.ToString(@"mm\:ss"),
                                "Sudoku Çözüldü",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }

                        return;
                    }
                }
            }
        }

        private void CheckUserInputs()
        {
            // Önce bütün kullanıcı hücrelerinin rengini sıfırla
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (!cells[row, col].ReadOnly)
                    {
                        cells[row, col].BackColor = Color.White;
                    }
                }
            }

            // Kullanıcının girdiği sayıları tekrar kontrol et
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (cells[row, col].ReadOnly)
                        continue;

                    if (string.IsNullOrWhiteSpace(cells[row, col].Text))
                        continue;

                    if (!int.TryParse(
                        cells[row, col].Text,
                        out int number))
                        continue;

                    if (!IsValidForScreen(row, col, number))
                    {
                        cells[row, col].BackColor = Color.LightCoral;
                    }
                }
            }
        }
        private bool IsSudokuCompleted()
        {
            // Önce bütün hücrelerin dolu olup olmadığını kontrol et
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (string.IsNullOrWhiteSpace(cells[row, col].Text))
                    {
                        return false;
                    }
                }
            }

            // Ekrandaki sayıları board'a aktar
            LoadBoardFromScreen();

            // Sudoku kurallarına uygun mu?
            return IsBoardValid();
        }
        private bool IsValidForScreen(
            int row,
            int col,
            int number)
        {
            // Satır
            for (int i = 0; i < 9; i++)
            {
                if (i == col)
                    continue;

                if (cells[row, i].Text == number.ToString())
                    return false;
            }

            // Sütun
            for (int i = 0; i < 9; i++)
            {
                if (i == row)
                    continue;

                if (cells[i, col].Text == number.ToString())
                    return false;
            }

            // 3x3 bölge
            int startRow = row - row % 3;
            int startCol = col - col % 3;

            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (i == row && j == col)
                        continue;

                    if (cells[i, j].Text == number.ToString())
                        return false;
                }
            }

            return true;
        }

        // --------------------------------------------------
        // YENİ OYUN
        // --------------------------------------------------

        private void NewGameButton_Click(object? sender, EventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            gameTimer.Stop();

            gameSeconds = 0;

            timeLabel.Text = "Oyun Süresi: 00:00";
            solveTimeLabel.Text = "Çözüm Süresi: -";
            statusLabel.Text = "Yeni Sudoku hazır!";

            ClearBoardData();

            FillBoard();

            RemoveNumbers();

            ShowBoard();

            gameTimer.Start();
        }

        // --------------------------------------------------
        // TAHTAYI TEMİZLE
        // --------------------------------------------------

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            gameTimer.Stop();

            gameSeconds = 0;

            timeLabel.Text = "Oyun Süresi: 00:00";
            solveTimeLabel.Text = "Çözüm Süresi: -";
            statusLabel.Text = "Tahta temizlendi.";

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    board[row, col] = 0;

                    cells[row, col].Text = "";
                    cells[row, col].ReadOnly = false;
                    cells[row, col].BackColor = Color.White;
                }
            }

            gameTimer.Start();
        }

        private void ClearBoardData()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    board[row, col] = 0;
                }
            }
        }

        // --------------------------------------------------
        // OYUN SÜRESİ
        // --------------------------------------------------

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            gameSeconds++;

            TimeSpan time =
                TimeSpan.FromSeconds(gameSeconds);

            timeLabel.Text =
                "Oyun Süresi: " +
                time.ToString(@"mm\:ss");
        }

        // --------------------------------------------------
        // SUDOKU ÇÖZ
        // --------------------------------------------------

        private void SolveButton_Click(object? sender, EventArgs e)
        {
            LoadBoardFromScreen();

            if (!IsBoardValid())
            {
                MessageBox.Show(
                    "Sudoku geçersiz!\n\n" +
                    "Aynı sayı satır, sütun veya 3x3 bölgede tekrar ediyor.",
                    "Hatalı Sudoku",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            solveStopwatch.Restart();

            if (SolveSudoku())
            {
                solveStopwatch.Stop();

                gameTimer.Stop();

                long milliseconds =
                    solveStopwatch.ElapsedMilliseconds;

                solveTimeLabel.Text =
                    "Çözüm Süresi: " +
                    milliseconds +
                    " ms";

                statusLabel.Text =
                    "Sudoku çözüldü!";

                ShowSolvedBoard();

                MessageBox.Show(
                    "Sudoku başarıyla çözüldü!\n\n" +
                    "Çözüm süresi: " +
                    milliseconds +
                    " ms",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                solveStopwatch.Stop();

                solveTimeLabel.Text =
                    "Çözüm Süresi: -";

                MessageBox.Show(
                    "Bu Sudoku çözülemiyor.",
                    "Çözüm Bulunamadı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // --------------------------------------------------
        // EKRANDAKİ VERİLERİ BOARD'A AKTAR
        // --------------------------------------------------

        private void LoadBoardFromScreen()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (int.TryParse(
                        cells[row, col].Text,
                        out int number))
                    {
                        board[row, col] = number;
                    }
                    else
                    {
                        board[row, col] = 0;
                    }
                }
            }
        }

        // --------------------------------------------------
        // SUDOKU GEÇERLİ Mİ?
        // --------------------------------------------------

        private bool IsBoardValid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                        continue;

                    int number = board[row, col];

                    board[row, col] = 0;

                    if (!IsValid(row, col, number))
                    {
                        board[row, col] = number;

                        return false;
                    }

                    board[row, col] = number;
                }
            }

            return true;
        }

        // --------------------------------------------------
        // SUDOKU KURALI
        // --------------------------------------------------

        private bool IsValid(
            int row,
            int col,
            int number)
        {
            // Satır
            for (int i = 0; i < 9; i++)
            {
                if (board[row, i] == number)
                    return false;
            }

            // Sütun
            for (int i = 0; i < 9; i++)
            {
                if (board[i, col] == number)
                    return false;
            }

            // 3x3 bölge
            int startRow = row - row % 3;
            int startCol = col - col % 3;

            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (board[i, j] == number)
                        return false;
                }
            }

            return true;
        }

        // --------------------------------------------------
        // BACKTRACKING SUDOKU ÇÖZÜCÜ
        // --------------------------------------------------

        private bool SolveSudoku()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        for (int number = 1; number <= 9; number++)
                        {
                            if (IsValid(row, col, number))
                            {
                                board[row, col] = number;

                                if (SolveSudoku())
                                    return true;

                                board[row, col] = 0;
                            }
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        // --------------------------------------------------
        // TAMAMLANMIŞ SUDOKU GÖSTER
        // --------------------------------------------------

        private void ShowSolvedBoard()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col].Text =
                        board[row, col].ToString();

                    cells[row, col].ReadOnly = true;

                    cells[row, col].BackColor =
                        Color.White;
                }
            }
        }

        // --------------------------------------------------
        // TAMAMLANMIŞ SUDOKU ÜRET
        // --------------------------------------------------

        private bool FillBoard()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        List<int> numbers =
                            Enumerable.Range(1, 9)
                            .OrderBy(x => random.Next())
                            .ToList();

                        foreach (int number in numbers)
                        {
                            if (IsValid(row, col, number))
                            {
                                board[row, col] = number;

                                if (FillBoard())
                                    return true;

                                board[row, col] = 0;
                            }
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        // --------------------------------------------------
        // SAYILARI SİL
        // --------------------------------------------------

        private void RemoveNumbers()
        {
            int cellsToRemove;

            switch (difficultyComboBox.SelectedItem?.ToString())
            {
                case "Kolay":
                    cellsToRemove = 35;
                    break;

                case "Zor":
                    cellsToRemove = 50;
                    break;

                default:
                    cellsToRemove = 43;
                    break;
            }

            while (cellsToRemove > 0)
            {
                int row = random.Next(0, 9);
                int col = random.Next(0, 9);

                if (board[row, col] != 0)
                {
                    board[row, col] = 0;

                    cellsToRemove--;
                }
            }
        }

        // --------------------------------------------------
        // TAHTAYI EKRANA YAZ
        // --------------------------------------------------

        private void ShowBoard()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        cells[row, col].Text = "";

                        cells[row, col].ReadOnly = false;

                        cells[row, col].BackColor =
                            Color.White;
                    }
                    else
                    {
                        cells[row, col].Text =
                            board[row, col].ToString();

                        cells[row, col].ReadOnly = true;

                        cells[row, col].BackColor =
                            Color.LightGray;
                    }
                }
            }
        }

        // --------------------------------------------------
        // FORM LOAD
        // --------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}