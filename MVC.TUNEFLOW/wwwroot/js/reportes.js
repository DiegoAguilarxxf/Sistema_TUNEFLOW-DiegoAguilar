// wwwroot/js/reportes.js

class ReporteImpresion {
    constructor(canvasId, titulo, datos, configExtra = {}) {
        this.canvasId = canvasId;
        this.titulo = titulo;
        this.datos = datos;
        this.configExtra = configExtra;
        this.chartInstance = null;
    }

    // Inicializa el gráfico
    initChart(type = 'line') {
        const ctx = document.getElementById(this.canvasId)?.getContext('2d');
        if (!ctx) return;

        const { labels, values } = this.datos;

        const defaultConfig = {
            type,
            data: {
                labels: labels,
                datasets: [{
                    label: this.titulo,
                    data: values,
                    borderColor: 'rgba(108, 99, 255, 1)',
                    backgroundColor: 'rgba(108, 99, 255, 0.2)',
                    fill: true,
                    tension: 0.3,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: 'rgba(108, 99, 255, 1)',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: this.titulo,
                        font: { size: 16 }
                    },
                    legend: {
                        display: true,
                        position: 'top'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0, stepSize: 1 },
                        title: { display: true, text: 'Cantidad' }
                    },
                    x: {
                        title: { display: true, text: 'Fecha' }
                    }
                },
                elements: {
                    point: { hoverRadius: 8 }
                }
            }
        };

        // Fusionar configuración extra
        const config = this.mergeDeep(defaultConfig, this.configExtra);
        this.chartInstance = new Chart(ctx, config);
    }

    // Función auxiliar para fusionar objetos profundamente
    mergeDeep(target, source) {
        const isObject = (obj) => obj && typeof obj === 'object';
        if (!isObject(target) || !isObject(source)) return source;
        Object.keys(source).forEach(key => {
            const targetValue = target[key];
            const sourceValue = source[key];
            if (Array.isArray(targetValue) && Array.isArray(sourceValue)) {
                target[key] = sourceValue;
            } else if (isObject(targetValue) && isObject(sourceValue)) {
                target[key] = this.mergeDeep({ ...targetValue }, sourceValue);
            } else {
                target[key] = sourceValue;
            }
        });
        return target;
    }

    // Imprimir como PDF
    imprimirPDF() {
        if (this.chartInstance) {
            const fecha = new Date().toISOString().slice(0, 10);
            const safeTitle = this.titulo.replace(/[^a-z0-9]/gi, '_').toLowerCase();
            document.title = `${safeTitle}_${fecha}`;

            window.print();

            setTimeout(() => {
                document.title = this.titulo;
            }, 1000);
        } else {
            alert('El gráfico aún no está listo. Por favor espera.');
        }
    }

    // Redimensionar antes de imprimir
    setupPrintEvents() {
        window.addEventListener('beforeprint', () => {
            if (this.chartInstance) this.chartInstance.resize();
        });
    }
}