const state = { categorias: [], productos: [] };
const $ = (selector) => document.querySelector(selector);

const money = new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 });
const number = new Intl.NumberFormat('es-CO');

async function api(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    headers: { 'Content-Type': 'application/json', ...(options.headers || {}) }
  });
  if (response.ok) return response.status === 204 ? null : response.json();
  let detail;
  try { detail = await response.json(); } catch { detail = {}; }
  const validation = detail.errors ? Object.values(detail.errors).flat().join(' ') : '';
  throw new Error(detail.mensaje || validation || `La solicitud falló (${response.status}).`);
}

function toast(message, error = false) {
  const el = $('#toast');
  el.textContent = message;
  el.className = `toast show${error ? ' error' : ''}`;
  clearTimeout(toast.timer);
  toast.timer = setTimeout(() => { el.className = 'toast'; }, 3200);
}

async function load() {
  try {
    [state.categorias, state.productos] = await Promise.all([
      api('/api/categorias'), api('/api/productos')
    ]);
    render();
  } catch (error) { toast(error.message, true); }
}

function render() {
  renderStats();
  renderCategoryOptions();
  renderProducts();
  renderCategories();
}

function renderStats() {
  const unidades = state.productos.reduce((sum, p) => sum + p.stock, 0);
  const valor = state.productos.reduce((sum, p) => sum + p.stock * p.precio, 0);
  $('#totalProductos').textContent = number.format(state.productos.length);
  $('#totalCategorias').textContent = number.format(state.categorias.length);
  $('#totalUnidades').textContent = number.format(unidades);
  $('#valorInventario').textContent = money.format(valor);
}

function renderCategoryOptions() {
  const options = state.categorias.map(c => `<option value="${c.id}">${escapeHtml(c.nombre)}</option>`).join('');
  const selectedFilter = $('#filtroCategoria').value;
  $('#filtroCategoria').innerHTML = `<option value="">Todas</option>${options}`;
  $('#filtroCategoria').value = selectedFilter;
  $('#productoCategoria').innerHTML = `<option value="" disabled selected>Selecciona una categoría</option>${options}`;
}

function renderProducts() {
  const term = $('#buscarProducto').value.trim().toLowerCase();
  const category = $('#filtroCategoria').value;
  const filtered = state.productos.filter(p =>
    (!term || p.nombre.toLowerCase().includes(term) || p.sku.toLowerCase().includes(term)) &&
    (!category || String(p.categoriaId) === category)
  );

  $('#productosBody').innerHTML = filtered.map(p => `
    <tr>
      <td><span class="product-name"><span class="product-avatar">□</span>${escapeHtml(p.nombre)}</span></td>
      <td><span class="sku">${escapeHtml(p.sku)}</span></td>
      <td><span class="badge">${escapeHtml(p.categoria)}</span></td>
      <td><strong>${money.format(p.precio)}</strong></td>
      <td><span class="stock${p.stock < 5 ? ' low' : ''}">${number.format(p.stock)}</span></td>
      <td class="actions"><button class="icon-button" data-edit-product="${p.id}" title="Editar">✎</button><button class="icon-button danger" data-delete-product="${p.id}" title="Eliminar">×</button></td>
    </tr>`).join('');
  $('#productosVacio').hidden = filtered.length > 0;
}

function renderCategories() {
  $('#categoriasGrid').innerHTML = state.categorias.map(c => `
    <article class="category-card">
      <header><span class="stat-icon blue">◇</span><span><button class="icon-button" data-edit-category="${c.id}" title="Editar">✎</button><button class="icon-button danger" data-delete-category="${c.id}" title="Eliminar">×</button></span></header>
      <h3>${escapeHtml(c.nombre)}</h3>
      <p>${escapeHtml(c.descripcion || 'Sin descripción')}</p>
      <footer><span>${number.format(c.cantidadProductos)} producto${c.cantidadProductos === 1 ? '' : 's'}</span><span>ID ${c.id}</span></footer>
    </article>`).join('');
}

function openProduct(product = null) {
  $('#productoForm').reset();
  $('#productoId').value = product?.id || '';
  $('#productoTitulo').textContent = product ? 'Editar producto' : 'Nuevo producto';
  if (product) {
    $('#productoNombre').value = product.nombre;
    $('#productoSku').value = product.sku;
    $('#productoPrecio').value = product.precio;
    $('#productoStock').value = product.stock;
    $('#productoCategoria').value = product.categoriaId;
  }
  $('#productoDialog').showModal();
}

function openCategory(category = null) {
  $('#categoriaForm').reset();
  $('#categoriaId').value = category?.id || '';
  $('#categoriaTitulo').textContent = category ? 'Editar categoría' : 'Nueva categoría';
  if (category) {
    $('#categoriaNombre').value = category.nombre;
    $('#categoriaDescripcion').value = category.descripcion || '';
  }
  $('#categoriaDialog').showModal();
}

$('#productoForm').addEventListener('submit', async (event) => {
  event.preventDefault();
  const id = $('#productoId').value;
  const body = {
    nombre: $('#productoNombre').value,
    sku: $('#productoSku').value,
    precio: Number($('#productoPrecio').value),
    stock: Number($('#productoStock').value),
    categoriaId: Number($('#productoCategoria').value)
  };
  try {
    await api(`/api/productos${id ? `/${id}` : ''}`, { method: id ? 'PUT' : 'POST', body: JSON.stringify(body) });
    $('#productoDialog').close();
    toast(id ? 'Producto actualizado.' : 'Producto creado.');
    await load();
  } catch (error) { toast(error.message, true); }
});

$('#categoriaForm').addEventListener('submit', async (event) => {
  event.preventDefault();
  const id = $('#categoriaId').value;
  const body = { nombre: $('#categoriaNombre').value, descripcion: $('#categoriaDescripcion').value || null };
  try {
    await api(`/api/categorias${id ? `/${id}` : ''}`, { method: id ? 'PUT' : 'POST', body: JSON.stringify(body) });
    $('#categoriaDialog').close();
    toast(id ? 'Categoría actualizada.' : 'Categoría creada.');
    await load();
  } catch (error) { toast(error.message, true); }
});

document.addEventListener('click', async (event) => {
  const target = event.target;
  if (target.id === 'nuevoProducto') openProduct();
  if (target.id === 'nuevaCategoria') openCategory();
  if (target.dataset.close) document.getElementById(target.dataset.close).close();
  if (target.dataset.editProduct) openProduct(state.productos.find(p => p.id === Number(target.dataset.editProduct)));
  if (target.dataset.editCategory) openCategory(state.categorias.find(c => c.id === Number(target.dataset.editCategory)));
  if (target.dataset.deleteProduct) {
    if (!confirm('¿Eliminar este producto? Esta acción no se puede deshacer.')) return;
    try { await api(`/api/productos/${target.dataset.deleteProduct}`, { method: 'DELETE' }); toast('Producto eliminado.'); await load(); }
    catch (error) { toast(error.message, true); }
  }
  if (target.dataset.deleteCategory) {
    if (!confirm('¿Eliminar esta categoría?')) return;
    try { await api(`/api/categorias/${target.dataset.deleteCategory}`, { method: 'DELETE' }); toast('Categoría eliminada.'); await load(); }
    catch (error) { toast(error.message, true); }
  }
});

$('#buscarProducto').addEventListener('input', renderProducts);
$('#filtroCategoria').addEventListener('change', renderProducts);

function escapeHtml(value) {
  return String(value).replace(/[&<>'"]/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;' })[char]);
}

load();
