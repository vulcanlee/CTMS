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

window.initDashboardCharts = function () {
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

    // Hospital Bar Chart
    const hospitalCtx = document.getElementById('hospitalChart').getContext('2d');
    const hospitalChart = new Chart(hospitalCtx, {
        type: 'bar',
        data: {
            labels: ['成大', '郭綜合', '奇美'],
            datasets: [{
                data: [0, 0, 0],
                backgroundColor: ['#4caf50', '#ff6b6b', '#ffc107'],
                borderRadius: 8,
                barThickness: 80
            }]
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
                    cornerRadius: 8
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max: 600,
                    grid: {
                        color: getGridColor(),
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: 150,
                        font: { size: 12 },
                        color: getTickColor()
                    }
                },
                x: {
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

    // Animate hospital bars one by one after a short delay
    const hospitalTargets = [402, 475, 285];
    hospitalTargets.forEach((target, i) => {
        setTimeout(() => {
            hospitalChart.data.datasets[0].data[i] = target;
            hospitalChart.update();
        }, i * 400);
    });

    // Stage Bar Chart
    const stageCtx = document.getElementById('stageChart').getContext('2d');
    const stageChart = new Chart(stageCtx, {
        type: 'bar',
        data: {
            labels: ['1期', '2期', '3期', '4期'],
            datasets: [{
                data: [0, 0, 0, 0],
                backgroundColor: ['#4caf50', '#ff6b6b', '#ffc107', '#42a5f5'],
                borderRadius: 8,
                barThickness: 70
            }]
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
                    cornerRadius: 8
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max: 600,
                    grid: {
                        color: getGridColor(),
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: 150,
                        font: { size: 12 },
                        color: getTickColor()
                    }
                },
                x: {
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

    // Animate stage bars one by one after a short delay
    const stageTargets = [380, 490, 315, 270];
    stageTargets.forEach((target, i) => {
        setTimeout(() => {
            stageChart.data.datasets[0].data[i] = target;
            stageChart.update();
        }, i * 400);
    });

    // Cancer Type Pie Chart
    const cancerTypeCtx = document.getElementById('cancerTypeChart').getContext('2d');
    const cancerTypeChart = new Chart(cancerTypeCtx, {
        type: 'doughnut',
        data: {
            labels: ['卵巢癌', '內膜癌'],
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
        cancerTypeChart.data.datasets[0].data = [524, 638];
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
        completionChart.data.datasets[0].data = [907, 255];
        completionChart.update();
    }, 300);
};

