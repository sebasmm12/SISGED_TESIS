
export function drawStackedColumnChart(data, xAxisLabel, yAxisLabel, legendType) {
    am5.ready(function () {
        var conditionalSeries = changeDataSeries(legendType);

        var convertedData = convertToColumnData(data, xAxisLabel, yAxisLabel);

        var root = "";
        if (legendType === "userDocumentsByState") {
            root = am5.Root.new("stacked-chart-user-documents-by-state");
        } else {
            root = am5.Root.new("stacked-chart");
        }

        root.setThemes([am5themes_Material.new(root)]);

        var chart = root.container.children.push(am5xy.XYChart.new(root,
            {
                panX: false,
                panY: false,
                wheelX: "panX",
                wheelY: "panY",
                paddingLeft: 0,
                layout: root.verticalLayout
            }));

        // Create axes
        var xRenderer = am5xy.AxisRendererX.new(root,
            {
                minorGridEnabled: true
            });

        var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root,
            {
                categoryField: xAxisLabel,
                renderer: xRenderer,
                tooltip: am5.Tooltip.new(root, {})
            }));

        xRenderer.grid.template.setAll({
            location: 0.5
        });

        xAxis.data.setAll(convertedData);

        var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root,
            {
                min: 1,
                renderer: am5xy.AxisRendererY.new(root,
                    {
                        strokeOpacity: 0.1
                    })

            }));

        // Add legends
        var legend = chart.children.push(am5.Legend.new(root,
            {
                nameField: "valueY",
                centerX: am5.p50,
                x: am5.p50
            }));

        // Add series
        function makeSeries(name, fieldName, color) {
            var series = chart.series.push(am5xy.ColumnSeries.new(root,
                {
                    name: name,
                    stacked: true,
                    xAxis: xAxis,
                    yAxis: yAxis,
                    valueYField: fieldName,
                    categoryXField: xAxisLabel
                }));

            series.columns.template.setAll(
                {
                    width: am5.percent(20),
                    tooltipText: "{name} : {valueY}",
                    tooltipY: am5.percent(10),
                    fill: color,
                    stroke: color
                });


            series.data.setAll(convertedData);

            series.appear();

            legend.data.push(series);
        }

        function fillSeries(yAxisLabel) {

            var dataSeries = conditionalSeries;

            var filteredSeries = dataSeries.find(series => series.name === yAxisLabel);

            filteredSeries.fields.forEach(field => {
                makeSeries(field.value, field.name, field.color);
            });

            // legend.data.setAll(filteredSeries.fields);
        }

        fillSeries(yAxisLabel);

        chart.appear(1000, 100);


    });
}


function convertToColumnData(data, xAxisLabel, yAxisLabel) {
    var result = [];

    data.forEach(element => {

        var convertedData = {};

        convertedData[xAxisLabel] = element[xAxisLabel];

        Object.keys(element[yAxisLabel]).forEach(key => {
            convertedData[key] = element[yAxisLabel][key];
        });

        result.push(convertedData);

    });

    return result;
}

function changeDataSeries(legendType) {
    if (legendType === "userDocumentsByState") {
        return [
            {
                name: "documents",
                fields: [
                    {
                        name: "registrado",
                        value: "Registrado",
                        color: am5.color("#2196F3")
                    },
                    {
                        name: "modificado",
                        value: "Modificado",
                        color: am5.color("#FFCA28")
                    },
                    {
                        name: "generado",
                        value: "Generado",
                        color: am5.color("#25393D")
                    },
                    {
                        name: "aprobado",
                        value: "Aprobado",
                        color: am5.color("#43A047")
                    },
                    {
                        name: "rechazado",
                        value: "Rechazado",
                        color: am5.color("#E53935")
                    },
                    {
                        name: "derivado",
                        value: "Derivado",
                        color: am5.color("#9C27B0")
                    },
                    {
                        name: "retrasado",
                        value: "Retrasado",
                        color: am5.color("#FF9800")
                    }
                ]
            }
        ];
    } else {
        return [
            {
                name: "documents",
                fields: [
                    {
                        name: "registrado",
                        value: "Registrado",
                        color: am5.color("#2196F3")
                    },
                    {
                        name: "modificado",
                        value: "Modificado",
                        color: am5.color("#FFCA28")
                    },
                    {
                        name: "generado",
                        value: "Generado",
                        color: am5.color("#25393D")
                    },
                    {
                        name: "aprobado",
                        value: "Aprobado",
                        color: am5.color("#43A047")
                    },
                    {
                        name: "rechazado",
                        value: "Rechazado",
                        color: am5.color("#E53935")
                    },
                    {
                        name: "derivado",
                        value: "Derivado",
                        color: am5.color("#9C27B0")
                    },
                    {
                        name: "retrasado",
                        value: "Retrasado",
                        color: am5.color("#FF9800")
                    }
                ]
            }
        ];
    }
}