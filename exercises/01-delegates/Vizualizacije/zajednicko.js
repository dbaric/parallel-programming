// Zajednička navigacija i pomoćne funkcije za sve stranice.
(function () {
  const pages = [
    ['index.html', 'Tjedan 1'],
    ['paralelizam.html', 'Paralelizam'],
    ['delegati.html', 'Delegati'],
    ['amdahl.html', 'Amdahl'],
    ['../../Tjedan%2002%20-%20Taskovi%20i%20zamke/Vizualizacije/index.html', 'Tjedan 2 →']
  ];
  const cur = decodeURIComponent(location.pathname.split('/').pop()) || 'index.html';
  const h = document.createElement('header');
  h.innerHTML = '<div class="wrap"><nav>' +
    pages.map(([u, t]) => `<a href="${u}"${u === cur ? ' class="cur"' : ''}>${t}</a>`).join('') +
    '</nav></div>';
  document.body.prepend(h);
})();
window.$ = id => document.getElementById(id);
