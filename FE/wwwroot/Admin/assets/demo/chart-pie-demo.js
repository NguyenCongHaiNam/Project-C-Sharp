// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#292b2c';

countD = 0;
countS = 0;

Datas.forEach(function(data) {
  var jsonData = JSON.parse(data);
  if(jsonData.predicted_label == "Danger news"){
    countD++;
  }else{
    countS++;
  }
});

var ctx = document.getElementById("myPieChart");
var myPieChart = new Chart(ctx, {
  type: 'pie',
  data: {
    labels: ["Danger news","Safe news"],
    datasets: [{
      data: [countD,countS],
      backgroundColor: ['#dc3545', '#28a745'],
    }],
  },
});
