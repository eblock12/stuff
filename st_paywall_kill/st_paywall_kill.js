var paywallElements = document.getElementsByClassName("hard_paywall");
for (var i = 0; i < paywallElements.length; i++)
{
  var element = paywallElements[i];
  element.parentElement.removeChild(element);
}

document.body.className = document.body.className.replace("noscroll", "")