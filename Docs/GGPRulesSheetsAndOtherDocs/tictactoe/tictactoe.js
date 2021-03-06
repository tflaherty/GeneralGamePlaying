//------------------------------------------------------------------------------
// tictactoe
//------------------------------------------------------------------------------

function renderstate (state)
 {var table = document.createElement('table');
  table.setAttribute('cellspacing','0');
  table.setAttribute('bgcolor','white');
  table.setAttribute('border','10');
  makerow(table,0,state);
  makerow(table,1,state);
  makerow(table,2,state);
  return table}

function makerow (table,rownum,state)
 {var row =table.insertRow(rownum);
  makecell(row,rownum,0,state);
  makecell(row,rownum,1,state);
  makecell(row,rownum,2,state);
  return row}

function makecell (row,rownum,colnum,state)
 {var cell = row.insertCell(colnum);
  cell.setAttribute('width','40');
  cell.setAttribute('height','40');
  cell.setAttribute('align','center');
  cell.setAttribute('valign','center');
  cell.setAttribute('style','font-family:helvetica;font-size:18pt');
  rownum = (rownum+1).toString();
  colnum = (colnum+1).toString();
  var mark = viewfindx('Z',seq('cell',rownum,colnum,'Z'),state,seq());
  if (mark && mark != 'b') {cell.innerHTML = mark} else {cell.innerHTML = '&nbsp;'};
  return cell}

//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
