using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ordenacions
{
    class MetodeOrdenacio
    {
        public static void SeleccioDirecte(int[] taula)
        {
            int i, j, min;
            for (i = 0; i < taula.Length - 1; i++)
            {
                min = i;
                for (j = i + 1; j < taula.Length; j++)
                {
                    if (taula[j] < taula[min])
                        min = j;
                }
                if (min != i)
                {
                    MainWindow.ModificarElementsGrafic(i, min);
                }
            }
        }

        public static void Bombolla(int[] taula)
        {
            for (int a = 1; a < taula.Length; a++)
                for (int b = taula.Length - 1; b >= a; b--)
                {
                    if (taula[b - 1] > taula[b])
                    {
                        MainWindow.ModificarElementsGrafic(b - 1, b);
                    }
                }
        }

        public static void Sacsejada(int[] taula)
        {
            int izquierda = 1;
            int derecha = taula.Length;
            int ultimo = taula.Length - 1;
            do
            {
                for (int i = taula.Length - 1; i > 0; i--)
                {
                    if (taula[i - 1] > taula[i])
                    {
                        MainWindow.ModificarElementsGrafic(i-1, i);
                        ultimo = i;
                    }
                }
                izquierda = ultimo + 1;
                for (int j = 1; j < taula.Length; j++)
                {
                    if (taula[j - 1] > taula[j])
                    {
                        MainWindow.ModificarElementsGrafic(j-1, j);
                        ultimo = j;
                    }
                }
                derecha = ultimo - 1;
            } while (derecha >= izquierda);
        }

        public static void Shell(int[] taula)
        {
            int salto = 0;
            int sw = 0;
            int e = 0;
            salto = taula.Length / 2;
            while (salto > 0)
            {
                sw = 1;
                while (sw != 0)
                {
                    sw = 0;
                    e = 1;
                    while (e <= (taula.Length - salto))
                    {
                        if (taula[(e - 1)+salto]< taula[e - 1])
                        {
                            MainWindow.ModificarElementsGrafic(e - 1, (e - 1)+salto);
                            sw = 1;
                        }
                        e++;
                    }
                }
                salto = salto / 2;
            }
        }

        public static void Quicksort(int[] taula)
        {
            Quicksort(taula, 0, taula.Length - 1);
        }
        private static void Quicksort(int[] taula, int primero, int ultimo)
        {
            int i, j, central;
            double pivote;
            central = (primero + ultimo) / 2;
            pivote = taula[central];
            i = primero;
            j = ultimo;
            do
            {
                while (taula[i] < pivote) i++;
                while (taula[j] > pivote) j--;
                if (i <= j)
                {
                    if(i!=j)
                        MainWindow.ModificarElementsGrafic(i, j);
                    i++;
                    j--;
                }
            } while (i <= j);

            if (primero < j)
            {
                Quicksort(taula, primero, j);
            }
            if (i < ultimo)
            {
                Quicksort(taula, i, ultimo);
            }
        }
     }
}
