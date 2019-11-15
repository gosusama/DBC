/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	config.language = 'vi';
	config.toolbarCanCollapse = true;
	config.extraPlugins = 'uploadimage';
	config.filebrowserUploadUrl = "/BTS.SP.IFT.CMS/controllers/Upload.ashx?type=posts&singleUpload=1";
};
