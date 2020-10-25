// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// get the canva element
var ctx = document.getElementById('myChart').getContext('2d');
// get the id of the values to be used
let f = document.getElementById('female').getAttribute("value");
let m = document.getElementById('male').getAttribute("value");

// implementation of chartjs syntax
var myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ["Female", "Male"],
        datasets: [{
            label: 'Gender Distribution',
            data: [f, m],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)'
            ],
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        }
    }
});