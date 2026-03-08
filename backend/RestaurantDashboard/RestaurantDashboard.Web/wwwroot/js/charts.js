/* Restaurant Dashboard — Chart.js helpers */

(function () {
    function hexToRgba(hex, alpha) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return 'rgba(' + r + ',' + g + ',' + b + ',' + alpha + ')';
    }

    const PALETTE = ['#10b981', '#3b82f6', '#ef4444', '#f59e0b', '#8b5cf6', '#06b6d4', '#f97316', '#ec4899'];

    window.renderBarChart = function (canvasId, labels, data, currencyLabel) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (canvas._chartInstance) {
            canvas._chartInstance.destroy();
            canvas._chartInstance = null;
        }

        const colors = PALETTE.slice(0, data.length);
        const bgColors = colors.map(function (c) { return hexToRgba(c, 0.75); });

        canvas._chartInstance = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: currencyLabel || '',
                    data: data,
                    backgroundColor: bgColors,
                    borderColor: colors,
                    borderWidth: 1,
                    borderRadius: 8,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 600 },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        backgroundColor: '#1a1f2e',
                        borderColor: '#252d3d',
                        borderWidth: 1,
                        titleColor: '#f1f5f9',
                        bodyColor: '#94a3b8',
                        padding: 10,
                        callbacks: {
                            label: function (ctx) {
                                return '  \u20AC' + Number(ctx.parsed.y).toFixed(2);
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { color: 'rgba(37,45,61,0.7)' },
                        ticks: { color: '#94a3b8', font: { size: 12 } },
                        border: { color: '#252d3d' }
                    },
                    y: {
                        grid: { color: 'rgba(37,45,61,0.7)' },
                        ticks: {
                            color: '#94a3b8',
                            font: { size: 12 },
                            callback: function (val) { return '\u20AC' + val.toLocaleString('en-IE'); }
                        },
                        border: { color: '#252d3d' }
                    }
                }
            }
        });
    };
})();
