// Zajednička navigacija i pomoćne funkcije za sve stranice.
(function () {
  const pages = [
    ['../../Tjedan%2004%20-%20Data%20race/Vizualizacije/index.html', '← Tjedan 4'],
    ['index.html', 'Tjedan 5'],
    ['glava.html', 'Glava liste i ograda'],
    ['modeli.html', 'Modeli i cijena ograde'],
    ['async.html', 'async/await']
  ];
  const cur = decodeURIComponent(location.pathname.split('/').pop()) || 'index.html';
  const h = document.createElement('header');
  h.innerHTML = '<div class="wrap"><nav>' +
    pages.map(([u, t]) => `<a href="${u}"${u === cur ? ' class="cur"' : ''}>${t}</a>`).join('') +
    '</nav></div>';
  document.body.prepend(h);
})();
window.$ = id => document.getElementById(id);
// Decimalni zarez i točka za tisućice, kao u hrvatskom tekstu.
window.hr = (x, d = 0) => {
  const [a, b] = Math.abs(x).toFixed(d).split('.');
  return (x < 0 ? '−' : '') + a.replace(/\B(?=(\d{3})+$)/g, '.') + (b ? ',' + b : '');
};
