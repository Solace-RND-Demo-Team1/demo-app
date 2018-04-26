mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
    console.log(str);
  },

});