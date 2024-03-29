function editMode() {
    var pass = document.getElementById('password');
    pass.disabled = !pass.disabled;
    var submitButton = document.getElementById('sbtBtn');
    var ResetButton = document.getElementById('resetbtn');
    submitButton.style.display = (pass.disabled) ? 'none' : 'inline';
    ResetButton.style.display = 'none';
}
function editMode1() {
    var uname = document.getElementById('userName');
    var role = document.getElementById('role');
    var status = document.getElementById('status');
    uname.disabled = !uname.disabled;
    role.disabled = !role.disabled;
    status.disabled = !status.disabled;
    var submitButton = document.getElementById('sbtBtn1');
    var cancelButton = document.getElementById('cancel');
    var editButton = document.getElementById('edtBtn1');
    submitButton.style.display = (uname.disabled || role.disabled || status.disabled) ? 'none' : 'inline';
    cancelButton.style.display = (uname.disabled || role.disabled || status.disabled) ? 'none' : 'inline';
    editButton.style.display = 'none';
}
function editMode2() {
    var fname = document.getElementById('firstName');
    var lname = document.getElementById('lastName');
    var email = document.getElementById('email');
    var sync = document.getElementById('syncemail');
    var mobile = document.getElementById('mobile');
    var medical = document.getElementById('mdLicense');
    var npi = document.getElementById('npi');
    fname.disabled = !fname.disabled;
    lname.disabled = !lname.disabled;
    email.disabled = !email.disabled;
    sync.disabled = !sync.disabled;
    mobile.disabled = !mobile.disabled;
    medical.disabled = !medical.disabled;
    npi.disabled = !npi.disabled;
    var submitButton = document.getElementById('sbtBtn2');
    var editButton = document.getElementById('edtBtn2');
    submitButton.style.display = (fname.disabled || lname.disabled || email.disabled || sync.disabled || mobile.disabled || medical.disabled || npi.disabled) ? 'none' : 'inline';
    editButton.style.display = 'none';
}
function editMode3() {
    var a1 = document.getElementById('address1');
    var a2 = document.getElementById('address2');
    var c = document.getElementById('city');
    var s = document.getElementById('state');
    var z = document.getElementById('zipCode');
    var m = document.getElementById('mobile2');
    a1.disabled = !a1.disabled;
    a2.disabled = !a2.disabled;
    c.disabled = !c.disabled;
    s.disabled = !s.disabled;
    z.disabled = !z.disabled;
    m.disabled = !m.disabled;
    var submitButton = document.getElementById('sbtBtn3');
    var editButton = document.getElementById('edtBtn3');
    submitButton.style.display = (a1.disabled || a2.disabled || c.disabled || s.disabled || z.disabled || m.disabled) ? 'none' : 'inline';
    editButton.style.display = 'none';
}
function editMode4() {
    var bn = document.getElementById('businessName');
    var bw = document.getElementById('businessWebsite');
    var file = document.getElementById('file');
    var sign = document.getElementById('sign');
    var create = document.getElementById('create');
    var notes = document.getElementById('notes');
    bn.disabled = !bn.disabled;
    bw.disabled = !bw.disabled;
    file.disabled = !file.disabled;
    sign.disabled = !sign.disabled;
    create.disabled = !create.disabled;
    notes.disabled = !notes.disabled;
    var submitButton = document.getElementById('sbtBtn4');
    var editButton = document.getElementById('edtBtn4');
    submitButton.style.display = (bn.disabled || bw.disabled || file.disabled || sign.disabled || create.disabled || notes.disabled) ? 'none' : 'inline';
    editButton.style.display = 'none';
}
function toggleViewButton(checkboxId, viewButtonId) {
    var checkbox = document.getElementById(checkboxId);
    var viewButton = document.getElementById(viewButtonId);
   
        if (checkbox.checked) {
            viewButton.style.display = 'inline-block';
        } else {
            viewButton.style.display = 'none';
        }
   
}
toggleViewButton('Agreement', 'ViewAgreement');
toggleViewButton('Background', 'ViewBackground');
toggleViewButton('HIPAA', 'ViewHIPAA');
toggleViewButton('NonDisclosure', 'ViewNonDisclosure');
toggleViewButton('License', 'ViewLicense');
