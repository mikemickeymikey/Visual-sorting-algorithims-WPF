using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace Ordenacions
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int[] taula;
        private static Rectangle[] taulaGrafic;
        private static CheckBox checkMarcaIntercanvi, checkMarcaAnimacioPosicio, checkMarcaAnimacioAlcada;
        private static ComboBox comboBoxAnimacions, comboBoxEasingMode;
        private static SolidColorBrush pinzellCorrecte= new SolidColorBrush();
        private static SolidColorBrush pinzellIncorrecte = new SolidColorBrush();
        private static SolidColorBrush pinzellIntercaniv= new SolidColorBrush();
        private SolidColorBrush pinzellFons = new SolidColorBrush();
        private static bool sonPunts = false;
        private static double alcadaRectangle;
        private static int segonsEsperar;
        private static bool atura = false;
        private bool inacabat = false;
        private static int zindex = 2;

        public MainWindow()
        {
            InitializeComponent();
            pinzellCorrecte.Color = (Color)ColorCorrecte.SelectedColor;
            pinzellIncorrecte.Color = (Color)ColorIncorrecte.SelectedColor;
            pinzellIntercaniv.Color = (Color)ColorIntercaniv.SelectedColor;
            pinzellFons.Color = (Color)ColorFons.SelectedColor;
            checkMarcaIntercanvi = CheckMarcaIntercaniv;
            checkMarcaAnimacioPosicio = CheckMarcaAnimacioPosicio;
            checkMarcaAnimacioAlcada = CheckMarcaAnimacioAlcada;
            comboBoxAnimacions = ComboBoxAnimacions;
            comboBoxEasingMode = ComboBoxEasingMode;
            CanvasGrafic.Background = pinzellFons;
            if (IntegTempsPausa.Value != null) segonsEsperar = (int)IntegTempsPausa.Value*30;
        }

        private void btGenera_Click(object sender, RoutedEventArgs e)
        {
            if (!inacabat)
            {
                GeneraTaula();
                GenerarGrafic();
            }
        }

        private void GenerarGrafic()
        {
            CanvasGrafic.Children.Clear();
            int gruixMarc = 0;
            int radiColumnes = 0;
            if (IntegTamanyMarc.Value != null) gruixMarc = (int)IntegTamanyMarc.Value*50;
            if (IntegRadiColumnes.Value != null) radiColumnes = (int)IntegRadiColumnes.Value*50;

            double ampladaRectangle = CanvasGrafic.Width / taula.Length;
            alcadaRectangle = CanvasGrafic.Height / taula.Length;
            double posicioCanvas = 0;

            Rectangle rec;
            if (ComboBoxTipusFigures.SelectedIndex == 1)
            #region generar Punts
            {
                sonPunts = true;
                radiColumnes = 10000;
                List<double> camps;
                for (int i = 0; i < taula.Length; i++)
                {
                    camps = new List<double>();
                    camps.Add(alcadaRectangle * taula[i]-alcadaRectangle);
                    camps.Add((i + 1) * alcadaRectangle-alcadaRectangle);
                    rec = new Rectangle()
                    {
                        Width = ampladaRectangle,
                        Height = alcadaRectangle,
                        Tag = camps,
                        Stroke = Brushes.Black,
                        StrokeThickness = gruixMarc,
                        RadiusX = radiColumnes,
                        RadiusY = radiColumnes,
                    };
                    PosicioCorrecte(rec);
                    taulaGrafic[i] = rec;
                    Canvas.SetBottom(rec, camps[0]);
                    Canvas.SetLeft(rec, posicioCanvas);
                    Canvas.SetZIndex(rec, 1);
                    CanvasGrafic.Children.Add(rec);
                    posicioCanvas += ampladaRectangle;
                }

            }
            #endregion
            else
            #region generar Rectangles
            {
                sonPunts = false;
                for (int i = 0; i < taula.Length; i++)
                {
                    rec = new Rectangle()
                    {
                        Width = ampladaRectangle,
                        Height = Math.Round(alcadaRectangle * taula[i], 5),
                        Tag = Math.Round((i + 1) * alcadaRectangle, 5),
                        Stroke = Brushes.Black,
                        StrokeThickness = gruixMarc,
                        RadiusX = radiColumnes,
                        RadiusY = radiColumnes,
                    };
                    PosicioCorrecte(rec);
                    taulaGrafic[i] = rec;
                    Canvas.SetBottom(rec, 0);
                    Canvas.SetLeft(rec, posicioCanvas);
                    Canvas.SetZIndex(rec, 1);
                    CanvasGrafic.Children.Add(rec);
                    posicioCanvas += ampladaRectangle;
                }
            }
            #endregion
        }

        private void GeneraTaula()
        {
            if (IntegNumeroElements.Value != null)
            {
                taulaGrafic = new Rectangle[(int)IntegNumeroElements.Value];
                taula = new int[(int)IntegNumeroElements.Value];
                if (RadioButtonInvertit.IsChecked == true)
                {
                    int num=taula.Length;
                    for (int i = 0; i < taula.Length; i++)
                    {
                        taula[i] = num;
                        num--;
                    }
                }
                else if (RadioButtonAleatori.IsChecked == true)
                {
                    Random r = new Random();
                    for (int i = 0; i < taula.Length; i++) { taula[i] = i + 1; }
                    int numeroRandom1, numeroRandom2, temp;
                    for (int i = 0; i < taula.Length; i++)
                    {
                        numeroRandom1 = r.Next(0, taula.Length);
                        numeroRandom2 = r.Next(0, taula.Length);
                        temp=taula[numeroRandom1];
                        taula[numeroRandom1] = taula[numeroRandom2];
                        taula[numeroRandom2] = temp;
                    }
                }
            }
        }

        public static void ModificarElementsGrafic(int posicio1, int posicio2)
        {
            Rectangle element1 = taulaGrafic[posicio1];
            Rectangle element2 = taulaGrafic[posicio2];
            double dada1=0, dada2=0;

            int valor2 = taula[posicio2];
            int valor1 = taula[posicio1];
            taula[posicio1] = valor2;
            taula[posicio2] = valor1;

            if (sonPunts)
            {
                dada1 = alcadaRectangle * valor2 - alcadaRectangle;
                ((List<double>)element1.Tag)[0] = dada1;
                Canvas.SetBottom(element1, dada1);
                dada2 = alcadaRectangle * valor1 - alcadaRectangle;
                ((List<double>)element2.Tag)[0] = dada2;
                Canvas.SetBottom(element2, dada2);
            }
            else
            {
                element1.Height = Math.Round(valor2 * alcadaRectangle, 5);
                element2.Height = Math.Round(valor1 * alcadaRectangle, 5);
            }

            if (checkMarcaIntercanvi.IsChecked == true)
            {
                element1.Fill = pinzellIntercaniv;
                element2.Fill = pinzellIntercaniv;
                if (sonPunts)
                    Animacio(element1, element2, valor1 < valor2, posicio1 - posicio2, dada1 - dada2);
                else
                    Animacio(element1, element2, valor1 < valor2, posicio1 - posicio2, element1.Height - element2.Height);
            }

            do
            {
                Espera(segonsEsperar);
            } while (atura);
            DoEvents();
            PosicioCorrecte(element1);
            PosicioCorrecte(element2);
        }

        private static void Animacio(Rectangle element1, Rectangle element2, bool creixent, int diferenciaPosicio, double diferenciaAlcada)
        {
            double amplada = element1.Width;
            double temps = segonsEsperar/1800.0;
            Canvas.SetZIndex(element1, zindex);
            Canvas.SetZIndex(element2, zindex);
            zindex++;
            if (diferenciaAlcada < 0)
                diferenciaAlcada = Math.Abs(diferenciaAlcada);

            #region definir animacions
            DoubleAnimation animacioPosicioElement1 = new DoubleAnimation();
            DoubleAnimation animacioAlcadaElement1 = new DoubleAnimation();
            DoubleAnimation animacioPosicioElement2 = new DoubleAnimation();
            DoubleAnimation animacioAlcadaElement2 = new DoubleAnimation();
            if (creixent)
            {
                animacioPosicioElement1.By = amplada * diferenciaPosicio;
                animacioAlcadaElement1.By = -diferenciaAlcada;
                animacioPosicioElement2.By = -amplada * diferenciaPosicio;
                animacioAlcadaElement2.By = diferenciaAlcada;
            }
            else
            {
                animacioPosicioElement1.By = -amplada * diferenciaPosicio;
                animacioAlcadaElement1.By = diferenciaAlcada;
                animacioPosicioElement2.By = amplada * diferenciaPosicio;
                animacioAlcadaElement2.By = -diferenciaAlcada;
            }

            //Tipus Animacions
            EasingFunctionBase easeElement1 = null;
            EasingFunctionBase easeElement2 = null;
            switch (comboBoxAnimacions.SelectedIndex)
            {
                case 1:
                    easeElement1 = new BackEase();
                    easeElement2 = new BackEase();
                    break;
                case 2:
                    easeElement1 = new ElasticEase();
                    easeElement2 = new ElasticEase();
                    break;
                case 3:
                    easeElement1 = new BounceEase();
                    easeElement2 = new BounceEase();
                    break;
                case 4:
                    easeElement1 = new CircleEase();
                    easeElement2 = new CircleEase();
                    break;
                case 5:
                    easeElement1 = new PowerEase();
                    easeElement2 = new PowerEase();
                    break;
            }

            if(easeElement1!=null && easeElement2 != null)
            {
                switch (comboBoxEasingMode.SelectedIndex)
                {
                    case 0:
                        easeElement1.EasingMode = EasingMode.EaseIn;
                        easeElement2.EasingMode = EasingMode.EaseIn;
                        break;
                    case 1:
                        easeElement1.EasingMode = EasingMode.EaseOut;
                        easeElement2.EasingMode = EasingMode.EaseOut;
                        break;
                    case 2:
                        easeElement1.EasingMode = EasingMode.EaseInOut;
                        easeElement2.EasingMode = EasingMode.EaseInOut;
                        break;
                }

                animacioPosicioElement1.EasingFunction = easeElement1;
                animacioPosicioElement2.EasingFunction = easeElement2;
            }


            animacioPosicioElement1.FillBehavior = FillBehavior.Stop;
            animacioPosicioElement2.FillBehavior = FillBehavior.Stop;
            animacioPosicioElement1.Duration = TimeSpan.FromSeconds(temps);
            animacioPosicioElement2.Duration = TimeSpan.FromSeconds(temps);
            animacioAlcadaElement1.FillBehavior = FillBehavior.Stop;
            animacioAlcadaElement2.FillBehavior = FillBehavior.Stop;
            animacioAlcadaElement1.Duration = TimeSpan.FromSeconds(temps);
            animacioAlcadaElement2.Duration = TimeSpan.FromSeconds(temps);
            #endregion

            #region assignar animacions
            Storyboard grupAnimacions = new Storyboard();
            Storyboard.SetTargetProperty(animacioPosicioElement1, new PropertyPath(LeftProperty));
            Storyboard.SetTarget(animacioPosicioElement1, element1);
            Storyboard.SetTargetProperty(animacioPosicioElement2, new PropertyPath(LeftProperty));
            Storyboard.SetTarget(animacioPosicioElement2, element2);

            if (sonPunts)
            {
                Storyboard.SetTargetProperty(animacioAlcadaElement1, new PropertyPath(Canvas.BottomProperty));
                Storyboard.SetTargetProperty(animacioAlcadaElement2, new PropertyPath(Canvas.BottomProperty));
            }
            else
            {
                Storyboard.SetTargetProperty(animacioAlcadaElement1, new PropertyPath(HeightProperty));
                Storyboard.SetTargetProperty(animacioAlcadaElement2, new PropertyPath(HeightProperty));
            }
            Storyboard.SetTarget(animacioAlcadaElement1, element1);
            Storyboard.SetTarget(animacioAlcadaElement2, element2);

            if (checkMarcaAnimacioPosicio.IsChecked == true)
            {
            grupAnimacions.Children.Add(animacioPosicioElement1);
            grupAnimacions.Children.Add(animacioPosicioElement2);
            }
            if (checkMarcaAnimacioAlcada.IsChecked == true)
            {
                grupAnimacions.Children.Add(animacioAlcadaElement1);
                grupAnimacions.Children.Add(animacioAlcadaElement2);
            }
            #endregion

            grupAnimacions.Begin();

        }
        private static void PosicioCorrecte(Rectangle rec)
        {
            if (sonPunts)
            {
                List<double> camps = (List<double>)rec.Tag;
                if (camps[0] == camps[1])
                    rec.Fill = pinzellCorrecte;
                else
                    rec.Fill = pinzellIncorrecte;
            }
            else
            {
                if (rec.Height == Convert.ToDouble(rec.Tag.ToString()))
                    rec.Fill = pinzellCorrecte;
                else
                    rec.Fill = pinzellIncorrecte;
            }
        }

        private void btOrdena_Click(object sender, RoutedEventArgs e)
        {
            if (taula != null && !inacabat)
            {
                inacabat = true;
                switch (ComboBoxTipusOrdenacio.SelectedIndex)
                {
                    case 0:
                        MetodeOrdenacio.SeleccioDirecte(taula);
                        break;
                    case 1:
                        MetodeOrdenacio.Bombolla(taula);
                        break;
                    case 2:
                        MetodeOrdenacio.Sacsejada(taula);
                        break;
                    case 3:
                        MetodeOrdenacio.Shell(taula);
                        break;
                    case 4:
                        MetodeOrdenacio.Quicksort(taula);
                        break;
                }
                inacabat = false;
            }
        }
        private void ActualitzarPinzells(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ColorPicker color = (ColorPicker)sender;
            if (color.SelectedColor != null)
            {
                if (color.Name.Equals("ColorCorrecte")) pinzellCorrecte.Color = (Color)color.SelectedColor;
                else if (color.Name.Equals("ColorIncorrecte")) pinzellIncorrecte.Color = (Color)color.SelectedColor;
                else if (color.Name.Equals("ColorIntercaniv")) pinzellIntercaniv.Color = (Color)color.SelectedColor;
                else if (color.Name.Equals("ColorFons")) pinzellFons.Color = (Color)color.SelectedColor;
            }
        }

        private void IntegTempsPausa_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IntegTempsPausa.Value != null) segonsEsperar = (int)IntegTempsPausa.Value*30;
        }

    #region Threads
    static Thread thread;
    private static void Espera(double milliseconds)
    {
        var frame = new DispatcherFrame();
        thread = new Thread((ThreadStart)(() =>
        {
                Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds));
                frame.Continue = false;
        }));
        thread.Start();
        Dispatcher.PushFrame(frame);
    }
    static Action action;

        public static void DoEvents()
    {
        action = new Action(delegate { });
        Application.Current?.Dispatcher?.Invoke(
           System.Windows.Threading.DispatcherPriority.Background,
           action);
    }

        protected override void OnClosed(EventArgs e)
    {
        Application.Current.Dispatcher.InvokeShutdown();
        thread?.Abort();
        base.OnClosed(e);
    }
        #endregion

        private void IntegNumeroElements_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IntegTamanyMarc!=null && IntegRadiColumnes!=null) {
                if (IntegNumeroElements.Value > 40)
                {
                    IntegTamanyMarc.Maximum = IntegNumeroElements.Value / 14;
                    IntegRadiColumnes.Maximum = IntegNumeroElements.Value / 14;
                    IntegTamanyMarc.Value = 4;
                    IntegRadiColumnes.Value = 2;
                }
                else
                {
                    IntegTamanyMarc.Maximum = 20;
                    IntegRadiColumnes.Maximum = 20;
                }
            }

        }


        private void ComboBoxAnimacions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxAnimacions != null)
            {
                if(comboBoxAnimacions.SelectedIndex==0)
                    comboBoxEasingMode.ItemsSource = new List<string> {""};
                else
                    comboBoxEasingMode.ItemsSource = new List<string> { "Easeln", "EaseOut", "EaseInOut" };
            }
        }


    }
}
