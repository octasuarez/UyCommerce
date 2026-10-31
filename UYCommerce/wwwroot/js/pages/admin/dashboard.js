
function LoadDashBoard(puntosVentas, puntosGanancias, lblVentas, lblGanancias){
    const ctx = document.getElementById("myChart");
    new Chart(ctx, {
      type: "line",
      data: {
        datasets: [
          {
            label: lblVentas,
            //data: [{ x: 'March', y: 20 }, { x: 'April', y: 10 }, { x: 'May', y: 5 },]
            data: puntosVentas,
          },
        ],
      },
      options: {
        scales: {
          y: {
            max: 10,
            beginAtZero: true,
            ticks: {
              precision: 0,
            },
          },
        },
      },
    });

    const ctx2 = document.getElementById("myChart2");

    new Chart(ctx2, {
      type: "bar",
      data: {
        datasets: [
          {
            label: lblGanancias,
            //data: [{ x: 'March', y: 20 }, { x: 'April', y: 10 }, { x: 'May', y: 5 },]
            data: puntosGanancias,
            backgroundColor: "lightgreen",
          },
        ],
      },
      options: {
        scales: {
          y: {
            //max: ,
            beginAtZero: true,
            ticks: {
              precision: 0,
            },
          },
        },
      },
    });

    $("form input").change(function () {
      $(this).closest("form").submit();
        });
}
