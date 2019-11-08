var passPhraseValidationResponse;

function initJQueryStepsWizard(containerRef, configDetails) {
  $(containerRef).steps({
    headerTag: configDetails.headerTag,
    bodyTag: configDetails.bodyTag,
    transitionEffect: "slideLeft",
    stepsOrientation: "vertical",

    /* Labels */
    labels: {
      cancel: "Cancel",
      current: "current step:",
      pagination: "Pagination",
      finish: "Finish",
      next: "Next",
      previous: "Previous",
      loading: "Loading ..."
    },

    /* Events */
    onStepChanging: function (event, currentIndex, newIndex) {
      if (currentIndex == 0) {
        if (
          !ValidatePassPhrase(
            $(configDetails.txtPassphrase).val(), configDetails.validateUrl)) {
        }
        return false;
      }
    }
  });
}

function ValidatePassPhrase(passPhrase, validateUrl) {
  var toRespond;
  $.ajax({
    type: "POST",
    url: validateUrl,
    data: { passPhrase: passPhrase },
    cache: false,
    success: function (data) {
      passPhraseValidationResponse = data;
      return true;
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert(
        "Server Error Occurred. Unable to verify passphrase. \n Message: " + thrownError);
      //Log(xhr.responseText);
    }
  });

  //return response;
}
function InitSmartWizard(containerID, formID, ajaxValidationConfig) {
  // Create extra buttons
  var btnFinish = $('<button></button>').text('Finish')
    .addClass('btn btn-info btn-finish disabled')
    .prop('disabled', true)
    .on('click', function () {
      if (!$(this).hasClass('disabled')) {
        var elmForm = $(formID);
        if (elmForm) {
          //elmForm.validator('validate');
          elmForm.validate();
          //var elmErr = elmForm.find('.has-error');
          if (!elmForm.valid()) {
            alert('Unable to save! Form has errors!');
            return false;
          } else {
            if (confirm('Create SuperAdmin?')) {
              elmForm.submit();
            }
            return false;
          }
        }
      }
    });
  var btnCancel = $('<button></button>').text('Cancel')
    .addClass('btn btn-danger')
    .on('click', function () {
      $(containerID).smartWizard("reset");
      $(formID).find("input, textarea").val("");
    });
  // Initialize the plugin
  $(containerID).smartWizard({
    selected: 0,  // Initial selected step, 0 = first step 
    keyNavigation: true, // Enable/Disable keyboard navigation(left and right keys are used if enabled)
    autoAdjustHeight: true, // Automatically adjust content height
    cycleSteps: false, // Allows to cycle the navigation of steps
    backButtonSupport: true, // Enable the back button support
    theme: 'arrows',
    transitionEffect: 'fade', // Effect on navigation, none/slide/fade
    transitionSpeed: '400',
    toolbarSettings: {
      toolbarPosition: 'bottom',
      toolbarExtraButtons: [btnFinish, btnCancel]
    },
    anchorSettings: {
      markDoneStep: true, // add done css
      markAllPreviousStepsAsDone: true, // When a step selected by url hash, all previous steps are marked done
      //removeDoneStepOnNavigateBack: true, // While navigate back done step after active step will be cleared
      enableAnchorOnDoneStep: true // Enable/Disable the done steps navigation
    }
  });

  // setup validation
  _SetupSmartWizardValidation(containerID, formID, ajaxValidationConfig);
}

function _SetupSmartWizardValidation(smartWizardContainerID, parentFormID, ajaxValidationConfig) {
  $(smartWizardContainerID).on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
    var parentForm = $(parentFormID);
    var elmForm = $("#form-step-" + stepNumber);
    //stepDirection === 'forward' :- this condition allows to do the form validation
    //only on forward navigation, that makes easy navigation on backwards still do the validation when going next
    if (stepDirection === 'forward' && elmForm) {
      parentForm.validate();
      if (!parentForm.valid()) {
        // Form validation failed
        return false;
      }
      // Form is valid, check the passphrase using ajax
      if (stepNumber == 0) {
        ValidatePassPhrase(
          $(ajaxValidationConfig.txtPassphrase).val(), ajaxValidationConfig.validateUrl);
        switch (passPhraseValidationResponse) {
          case "True":
            return true;
          case "False":
            return false;
        };
      }
    }
    // Exit normally
    return true;
  });
}

function SmartWizardEnableFinishButtonOnLastStep(smartWizardContainerID, lastStepNumber) {
  lastStepNumber -= 1;
  $(smartWizardContainerID).on("showStep", function (e, anchorObject, stepNumber, stepDirection) {
    // Enable finish button only on last step
    if (stepNumber == lastStepNumber) {
      $('.btn-finish').removeClass('disabled').prop('disabled', false);
    } else {
      $('.btn-finish').addClass('disabled').prop('disabled', true);
    }
  });
}