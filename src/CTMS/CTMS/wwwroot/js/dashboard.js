// Chart.js default settings (global scope so toggleTheme can access it)
function updateChartDefaults() {
    // 取得 id 為 dashboard-container 的元素
    const dashboardContainer = document.getElementById('dashboard-container');

    const isLightMode = document.getElementById('dashboard-container').classList.contains('light-mode');
    Chart.defaults.color = isLightMode ? '#bdc3c7' : '#999';
    Chart.defaults.font.family = "'Noto Sans TC', sans-serif";
}

// Helper functions (global scope)
function getGridColor() {
    return document.getElementById('dashboard-container').classList.contains('light-mode') ? '#f0f0f0' : '#4a4a4a';
}
function getBorderColor() {
    return document.getElementById('dashboard-container').classList.contains('light-mode') ? '#f0f0f0' : '#4a4a4a';
}
function getTickColor() {
    return document.getElementById('dashboard-container').classList.contains('light-mode') ? '#bdc3c7' : '#999';
}
function getTooltipBg() {
    return document.getElementById('dashboard-container').classList.contains('light-mode') ? '#ffffff' : '#1e1e1e';
}

// Theme Toggle Function
function toggleTheme() {
    const body = document.getElementById('dashboard-container');
    const themeIcon = document.querySelector('.theme-icon');

    body.classList.toggle('light-mode');

    if (body.classList.contains('light-mode')) {
        themeIcon.textContent = '🌙';
        localStorage.setItem('theme', 'light');
    } else {
        themeIcon.textContent = '☀️';
        localStorage.setItem('theme', 'dark');
    }
    //location.reload();
}

// Load saved theme on page load
// document.addEventListener('DOMContentLoaded', function() {
//     const savedTheme = localStorage.getItem('theme');
//     const themeIcon = document.querySelector('.theme-icon');

//     // Default to light mode
//     if (!savedTheme || savedTheme === 'light') {
//         document.body.classList.add('light-mode');
//         themeIcon.textContent = '🌙';
//         localStorage.setItem('theme', 'light');
//     }

//     // Animate stat values on page load
//     animateStatValues();
// });


// Function to animate stat values counting up
function animateStatValues() {
    const statValues = document.querySelectorAll('.stat-value');

    statValues.forEach((element, index) => {
        const text = element.textContent;
        // Extract numeric value (remove commas and % signs)
        const numericText = text.replace(/[,%]/g, '');
        const targetValue = parseFloat(numericText);

        if (isNaN(targetValue)) return;

        const isPercentage = text.includes('%');
        const hasComma = text.includes(',');
        const duration = 2000;
        const startTime = performance.now();
        const delay = index * 150;

        element.textContent = '0';

        function updateValue(currentTime) {
            const elapsed = currentTime - startTime - delay;

            if (elapsed < 0) {
                requestAnimationFrame(updateValue);
                return;
            }

            if (elapsed < duration) {
                const progress = elapsed / duration;
                const easedProgress = 1 - Math.pow(1 - progress, 4);
                const currentValue = targetValue * easedProgress;

                let displayValue;
                if (isPercentage) {
                    displayValue = currentValue.toFixed(1) + '%';
                } else if (hasComma) {
                    displayValue = Math.floor(currentValue).toLocaleString();
                } else {
                    displayValue = Math.floor(currentValue);
                }

                element.textContent = displayValue;
                requestAnimationFrame(updateValue);
            } else {
                element.textContent = text;
            }
        }

        requestAnimationFrame(updateValue);
    });
}

window.initDashboardCharts = function (viewModel) {
    // Initialize theme on load
    const savedTheme = localStorage.getItem('theme');
    const themeIcon = document.querySelector('.theme-icon');

    if (!savedTheme || savedTheme === 'light') {
        document.getElementById('dashboard-container').classList.add('light-mode');
        if (themeIcon) themeIcon.textContent = '🌙';
        localStorage.setItem('theme', 'light');
    } else if (savedTheme === 'dark') {
        document.getElementById('dashboard-container').classList.remove('light-mode');
        if (themeIcon) themeIcon.textContent = '☀️';
    }

    // Animate stat values on page load
    animateStatValues();

    updateChartDefaults();

    // 從 ViewModel 提取數據，如果沒有提供則使用默認值
    const hospitalLabels = viewModel?.hospitalStats?.map(h => h.hospitalName) || ['成大', '郭綜合', '奇美'];
    const hospitalAiData = viewModel?.hospitalStats?.map(h => h.aiCount) || [200, 240, 140];
    const hospitalControlData = viewModel?.hospitalStats?.map(h => h.controlCount) || [202, 235, 145];
    const hospitalMaxValue = Math.max(...hospitalAiData.map((ai, i) => ai + hospitalControlData[i])) * 1.2; // 動態計算最大值，給予 20% 的空間

    const stageLabels = viewModel?.stageStats?.map(s => s.stageName) || ['I', 'II', 'III', 'IV'];
    const stageAiData = viewModel?.stageStats?.map(s => s.aiCount) || [190, 245, 158, 135];
    const stageControlData = viewModel?.stageStats?.map(s => s.controlCount) || [190, 245, 157, 135];
    const stageMaxValue = Math.max(...stageAiData.map((ai, i) => ai + stageControlData[i])) * 1.2;

    const cancerTypeAiData = [
        viewModel?.cancerTypeStats?.ovarianAiCount || 0,
        viewModel?.cancerTypeStats?.endometrialAiCount || 0
    ];
    const cancerTypeControlData = [
        viewModel?.cancerTypeStats?.ovarianControlCount || 0,
        viewModel?.cancerTypeStats?.endometrialControlCount || 0
    ];
    const cancerTypeMaxValue = Math.max(...cancerTypeAiData.map((ai, i) => ai + cancerTypeControlData[i])) * 1.2 || 10;

    const completionData = [
        viewModel?.completionStats?.completedCount || 907,
        viewModel?.completionStats?.incompleteCount || 255
    ];

    // Hospital Stacked Bar Chart
    const hospitalCtx = document.getElementById('hospitalChart').getContext('2d');
    const hospitalChart = new Chart(hospitalCtx, {
        type: 'bar',
        data: {
            labels: hospitalLabels,
            datasets: [
                {
                    label: 'AI組',
                    data: new Array(hospitalLabels.length).fill(0),
                    backgroundColor: '#42a5f5',
                    borderRadius: 0,
                    barThickness: 60
                },
                {
                    label: '對照組',
                    data: new Array(hospitalLabels.length).fill(0),
                    backgroundColor: '#ff6b6b',
                    borderRadius: 8,
                    barThickness: 60
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 1800,
                easing: 'easeOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: getTooltipBg(),
                    titleColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    bodyColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    borderColor: '#e0e0e0',
                    borderWidth: 1,
                    padding: 12,
                    titleFont: { size: 14 },
                    bodyFont: { size: 13 },
                    cornerRadius: 8,
                    callbacks: {
                        footer: function(tooltipItems) {
                            const total = tooltipItems.reduce((sum, item) => sum + item.parsed.y, 0);
                            return '總計: ' + total;
                        }
                    }
                }
            },
            scales: {
                y: {
                    stacked: true,
                    beginAtZero: true,
                    max: hospitalMaxValue,
                    grid: {
                        color: getGridColor(),
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: Math.ceil(hospitalMaxValue / 4),
                        font: { size: 12 },
                        color: getTickColor()
                    }
                },
                x: {
                    stacked: true,
                    grid: {
                        display: false,
                        drawBorder: true,
                        borderColor: getBorderColor()
                    },
                    ticks: {
                        font: { size: 13 },
                        color: getTickColor()
                    },
                    border: {
                        color: getBorderColor()
                    }
                }
            }
        }
    });

    // Animate hospital stacked bars one by one after a short delay
    hospitalLabels.forEach((_, i) => {
        setTimeout(() => {
            hospitalChart.data.datasets[0].data[i] = hospitalAiData[i];
            hospitalChart.data.datasets[1].data[i] = hospitalControlData[i];
            hospitalChart.update();
        }, i * 400);
    });

    // Stage Stacked Bar Chart
    const stageCtx = document.getElementById('stageChart').getContext('2d');
    const stageChart = new Chart(stageCtx, {
        type: 'bar',
        data: {
            labels: stageLabels,
            datasets: [
                {
                    label: 'AI組',
                    data: new Array(stageLabels.length).fill(0),
                    backgroundColor: '#42a5f5',
                    borderRadius: 0,
                    barThickness: 60
                },
                {
                    label: '對照組',
                    data: new Array(stageLabels.length).fill(0),
                    backgroundColor: '#ff6b6b',
                    borderRadius: 8,
                    barThickness: 60
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 1800,
                easing: 'easeOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: getTooltipBg(),
                    titleColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    bodyColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    borderColor: '#e0e0e0',
                    borderWidth: 1,
                    padding: 12,
                    titleFont: { size: 14 },
                    bodyFont: { size: 13 },
                    cornerRadius: 8,
                    callbacks: {
                        footer: function(tooltipItems) {
                            const total = tooltipItems.reduce((sum, item) => sum + item.parsed.y, 0);
                            return '總計: ' + total;
                        }
                    }
                }
            },
            scales: {
                y: {
                    stacked: true,
                    beginAtZero: true,
                    max: stageMaxValue,
                    grid: {
                        color: getGridColor(),
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: Math.ceil(stageMaxValue / 4),
                        font: { size: 12 },
                        color: getTickColor()
                    }
                },
                x: {
                    stacked: true,
                    grid: {
                        display: false,
                        drawBorder: true,
                        borderColor: getBorderColor()
                    },
                    ticks: {
                        font: { size: 13 },
                        color: getTickColor()
                    },
                    border: {
                        color: getBorderColor()
                    }
                }
            }
        }
    });

    // Animate stage stacked bars one by one after a short delay
    stageLabels.forEach((_, i) => {
        setTimeout(() => {
            stageChart.data.datasets[0].data[i] = stageAiData[i];
            stageChart.data.datasets[1].data[i] = stageControlData[i];
            stageChart.update();
        }, i * 400);
    });

    // Cancer Type Stacked Bar Chart (卵巢癌 / 內膜癌, AI組 / 對照組)
    const cancerTypeCtx = document.getElementById('cancerTypeChart').getContext('2d');
    const cancerTypeChart = new Chart(cancerTypeCtx, {
        type: 'bar',
        data: {
            labels: ['卵巢癌', '內膜癌'],
            datasets: [
                {
                    label: 'AI組',
                    data: [0, 0],
                    backgroundColor: '#42a5f5',
                    borderRadius: 0,
                    barThickness: 60
                },
                {
                    label: '對照組',
                    data: [0, 0],
                    backgroundColor: '#ff6b6b',
                    borderRadius: 8,
                    barThickness: 60
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 1800,
                easing: 'easeOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: getTooltipBg(),
                    titleColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    bodyColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    borderColor: '#e0e0e0',
                    borderWidth: 1,
                    padding: 12,
                    titleFont: { size: 14 },
                    bodyFont: { size: 13 },
                    cornerRadius: 8,
                    callbacks: {
                        footer: function(tooltipItems) {
                            const total = tooltipItems.reduce((sum, item) => sum + item.parsed.y, 0);
                            return '總計: ' + total;
                        }
                    }
                }
            },
            scales: {
                y: {
                    stacked: true,
                    beginAtZero: true,
                    max: cancerTypeMaxValue,
                    grid: {
                        color: getGridColor(),
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: Math.ceil(cancerTypeMaxValue / 4),
                        font: { size: 12 },
                        color: getTickColor()
                    }
                },
                x: {
                    stacked: true,
                    grid: {
                        display: false,
                        drawBorder: true,
                        borderColor: getBorderColor()
                    },
                    ticks: {
                        font: { size: 13 },
                        color: getTickColor()
                    },
                    border: {
                        color: getBorderColor()
                    }
                }
            }
        }
    });

    setTimeout(() => {
        cancerTypeChart.data.datasets[0].data = cancerTypeAiData;
        cancerTypeChart.data.datasets[1].data = cancerTypeControlData;
        cancerTypeChart.update();
    }, 300);

    // Completion Pie Chart
    const completionCtx = document.getElementById('completionChart').getContext('2d');
    const completionChart = new Chart(completionCtx, {
        type: 'doughnut',
        data: {
            labels: ['已完成', '未完成'],
            datasets: [{
                data: [0, 0],
                backgroundColor: ['#ff6b6b', '#4caf50'],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '65%',
            animation: {
                animateRotate: true,
                animateScale: true,
                duration: 2000,
                easing: 'easeInOutQuart'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: getTooltipBg(),
                    titleColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    bodyColor: document.getElementById('dashboard-container').classList.contains('light-mode') ? '#2c3e50' : '#fff',
                    borderColor: '#e0e0e0',
                    borderWidth: 1,
                    padding: 12,
                    titleFont: { size: 14 },
                    bodyFont: { size: 13 },
                    cornerRadius: 8
                }
            }
        }
    });

    setTimeout(() => {
        completionChart.data.datasets[0].data = completionData;
        completionChart.update();
    }, 300);
};

