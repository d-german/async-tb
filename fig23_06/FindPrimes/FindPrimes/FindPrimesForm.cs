﻿// Fig. 23.6: FindPrimes.cs
// Displaying an asynchronous task's progress and intermediate results

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindPrimes
{
    public partial class FindPrimesForm : Form
    {
        private bool[] primes; // array used to determine primes

        public FindPrimesForm()
        {
            InitializeComponent();
            progressBar.Minimum = 2; // 2 is the smallest prime number
            percentageLabel.Text = $"{0:P0}"; // display 0 %
        }

        // used to enable cancelation of the async task
        private bool Canceled { get; set; }

        // handles getPrimesButton's click event
        private async void getPrimesButton_Click(object sender, EventArgs e)
        {
            // get user input
            var maximum = int.Parse(maxValueTextBox.Text);

            // create array for determining primes
            primes = Enumerable.Repeat(true, maximum).ToArray();

            // reset Canceled and GUI
            Canceled = false;
            getPrimesButton.Enabled = false; // disable getPrimesButton
            cancelButton.Enabled = true; // enable cancelButton
            primesTextBox.Text = string.Empty; // clear primesTextBox
            statusLabel.Text = string.Empty; // clear statusLabel
            percentageLabel.Text = $"{0:P0}"; // display 0 %
            progressBar.Value = progressBar.Minimum; // reset progressBar min
            progressBar.Maximum = maximum; // set progressBar max

            // show primes up to maximum
            var count = await FindPrimes1(maximum); 
            // var count = await FindPrimes2(maximum); 
            //var count = await Task.Run(() => FindPrimes3(maximum));

//            var dist = (int) Math.Floor((decimal) (maximum / 4));
//            const int min = 2;
//            var t1 = Task.Run(() => FindPrimes4(min, min + 1 * dist));
//            var t2 = Task.Run(() => FindPrimes4(min + dist, min + 2 * dist));
//            var t3 = Task.Run(() => FindPrimes4(min + dist, min + 3 * dist));
//            var t4 = Task.Run(() => FindPrimes4(min + dist, (4 * dist)));
//            var res = await Task.WhenAll(t1, t2, t3, t4);
//            var count = res.Sum();

            statusLabel.Text = $"Found {count} prime(s)";
        }

        // displays prime numbers in primesTextBox
        private async Task<int> FindPrimes1(int maximum)
        {
            var primeCount = 0;

            // find primes less than maximum
            for (var i = 2; i < maximum && !Canceled; ++i)
            {
                // if i is prime, display it
                if (await Task.Run(() => IsPrime(i)))
                {
                    ++primeCount; // increment number of primes found
                    primesTextBox.AppendText($"{i}{Environment.NewLine}");
                }

                var percentage = (double) progressBar.Value /
                                 (progressBar.Maximum - progressBar.Minimum + 1);
                percentageLabel.Text = $"{percentage:P0}";
                progressBar.Value = i + 1; // update progress
            }

            // display message if operation was canceled
            if (Canceled)
            {
                primesTextBox.AppendText($"Canceled{Environment.NewLine}");
            }

            getPrimesButton.Enabled = true; // enable getPrimesButton
            cancelButton.Enabled = false; // disable cancelButton
            return primeCount;
        }

        // displays prime numbers in primesTextBox
        private async Task<int> FindPrimes2(int maximum)
        {
            var primeCount = 0;

            // find primes less than maximum
            for (var i = 2; i < maximum && !Canceled; ++i)
            {
                // if i is prime, display it
                if (await Task.Run(() => IsPrime(i)))
                {
                    ++primeCount; // increment number of primes found
                    primesTextBox.AppendText($"{i}{Environment.NewLine}");
                }
                else
                {
                    await Task.Delay(1);
                    //Thread.Sleep(1);
                }

                var percentage = (double) progressBar.Value /
                                 (progressBar.Maximum - progressBar.Minimum + 1);
                percentageLabel.Text = $"{percentage:P0}";
                progressBar.Value = i + 1; // update progress
            }

            // display message if operation was canceled
            if (Canceled)
            {
                primesTextBox.AppendText($"Canceled{Environment.NewLine}");
            }

            getPrimesButton.Enabled = true; // enable getPrimesButton
            cancelButton.Enabled = false; // disable cancelButton
            return primeCount;
        }

        private async Task<int> FindPrimes3(int maximum)
        {
            var primeCount = 0;

            // find primes less than maximum
            for (var i = 2; i < maximum && !Canceled; ++i)
            {
                // if i is prime, display it
                if (await Task.Run(() => IsPrime(i)))
                {
                    Invoke(new MethodInvoker(() =>
                    {
                        ++primeCount; // increment number of primes found
                        primesTextBox.AppendText($"{i}{Environment.NewLine}");
                    }));
                }

                Invoke(new MethodInvoker(() =>
                {
                    var percentage = (double) progressBar.Value /
                                     (progressBar.Maximum - progressBar.Minimum + 1);
                    percentageLabel.Text = $"{percentage:P0}";
                    progressBar.Value = i + 1; // update progress
                }));
            }

            // display message if operation was canceled
            if (Canceled)
            {
                Invoke(new MethodInvoker(() => { primesTextBox.AppendText($"Canceled{Environment.NewLine}"); }));
            }

            Invoke(new MethodInvoker(() =>
            {
                getPrimesButton.Enabled = true; // enable getPrimesButton
                cancelButton.Enabled = false; // disable cancelButton
            }));

            return primeCount;
        }

        private async Task<int> FindPrimes4(int minimum, int maximum)
        {
            var primeCount = 0;

            // find primes less than maximum
            for (var i = minimum; i < maximum && !Canceled; ++i)
            {
                // if i is prime, display it
                if (await Task.Run(() => IsPrime(i)))
                {
                    Invoke(new MethodInvoker(() =>
                    {
                        ++primeCount; // increment number of primes found
                        primesTextBox.AppendText($"{i}{Environment.NewLine}");
                    }));
                }

                Invoke(new MethodInvoker(() =>
                {
                    var percentage = (double) progressBar.Value /
                                     (progressBar.Maximum - progressBar.Minimum + 1);
                    percentageLabel.Text = $"{percentage:P0}";
                    progressBar.Value = i + 1; // update progress
                }));
            }

            // display message if operation was canceled
            if (Canceled)
            {
                Invoke(new MethodInvoker(() => { primesTextBox.AppendText($"Canceled{Environment.NewLine}"); }));
            }

            Invoke(new MethodInvoker(() =>
            {
                getPrimesButton.Enabled = true; // enable getPrimesButton
                cancelButton.Enabled = false; // disable cancelButton
            }));

            return primeCount;
        }

        // check whether value is a prime number 
        // and mark all multiples as not prime
        public bool IsPrime(int value)
        {
            // if value is prime, mark all of multiples
            // as not prime and return true
            if (primes[value])
            {
                // mark all multiples of value as not prime
                for (var i = value + value; i < primes.Length; i += value)
                {
                    primes[i] = false; // i is not prime
                }

                return true;
            }

            return false;
        }

        // if user clicks Cancel Button, stop displaying primes
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Canceled = true;
            getPrimesButton.Enabled = true; // enable getPrimesButton
            cancelButton.Enabled = false; // disable cancelButton
        }
    }
}

/**************************************************************************
 * (C) Copyright 1992-2017 by Deitel & Associates, Inc. and               *
 * Pearson Education, Inc. All Rights Reserved.                           *
 *                                                                        *
 * DISCLAIMER: The authors and publisher of this book have used their     *
 * best efforts in preparing the book. These efforts include the          *
 * development, research, and testing of the theories and programs        *
 * to determine their effectiveness. The authors and publisher make       *
 * no warranty of any kind, expressed or implied, with regard to these    *
 * programs or to the documentation contained in these books. The authors *
 * and publisher shall not be liable in any event for incidental or       *
 * consequential damages in connection with, or arising out of, the       *
 * furnishing, performance, or use of these programs.                     *
 **************************************************************************/